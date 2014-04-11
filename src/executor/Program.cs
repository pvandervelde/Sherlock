//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Mono.Options;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Executor.Nuclei.ExceptionHandling;
using Sherlock.Executor.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    /// <summary>
    /// Defines the entry point for the executor application.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "executor.error.{0}.log";

        [STAThread]
        static int Main(string[] args)
        {
            int functionReturnResult = -1;

            var processor = new LogBasedExceptionProcessor(
                LoggerBuilder.ForFile(
                    Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultErrorFileName),
                    new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now)));
            var result = TopLevelExceptionGuard.RunGuarded(
                () => functionReturnResult = RunApplication(args),
                new ExceptionProcessor[]
                    {
                        processor.Process, 
                    });

            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're just catching and then exiting the app.")]
        private static int RunApplication(string[] arguments)
        {
            IContainer container = null;
            try
            {
                string hostIdText = null;
                string channelTypeText = null;
                string channelUriText = null;

                var options = new OptionSet 
                {
                    { 
                        Resources.CommandLine_Param_Host_Key, 
                        Resources.CommandLine_Param_Host_Description, 
                        v => hostIdText = v
                    },
                    {
                        Resources.CommandLine_Param_ChannelType_Key,
                        Resources.CommandLine_Param_ChannelType_Description,
                        v => channelTypeText = v
                    },
                    {
                        Resources.CommandLine_Param_ChannelUri_Key,
                        Resources.CommandLine_Param_ChannelUri_Description,
                        v => channelUriText = v
                    },
                };

                options.Parse(arguments);
                if (string.IsNullOrWhiteSpace(hostIdText) ||
                    string.IsNullOrWhiteSpace(channelTypeText) ||
                    string.IsNullOrWhiteSpace(channelUriText))
                {
                    return UnhandledExceptionApplicationExitCode;
                }

                var storageDirectory = Path.Combine(
                    Path.GetTempPath(),
                    Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
                var hostId = EndpointIdExtensions.Deserialize(hostIdText);
                var channelType = (ChannelType)Enum.Parse(typeof(ChannelType), channelTypeText);
                
                container = DependencyInjection.CreateContainer(storageDirectory, hostId);

                var diagnostics = container.Resolve<SystemDiagnostics>();
                diagnostics.Log(
                    LevelToLog.Debug,
                    ExecutorConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_ConnectingToHost_WithConnectionParameters,
                        hostId,
                        channelType,
                        channelUriText));
                
                var resolver = container.Resolve<ManualEndpointConnection>();
                resolver(hostId, channelType, channelUriText);

                WaitForHostToConnect(hostId, container);

                var result = ExecuteTest(hostId, storageDirectory, container);
                return (result == TestExecutionResult.Passed) 
                    ? NormalApplicationExitCode 
                    : Constants.TestsFailedApplicationExitCode;
            }
            finally
            {
                if (container != null)
                {
                    container.Dispose();
                }
            }
        }

        private static void WaitForHostToConnect(EndpointId hostId, IComponentContext container)
        {
            var diagnostics = container.Resolve<SystemDiagnostics>();
            var commands = container.Resolve<ISendCommandsToRemoteEndpoints>();

            var resetEvent = new AutoResetEvent(false);
            var notificationAvailabilityNotifier =
                Observable.FromEventPattern<CommandSetAvailabilityEventArgs>(
                    h => commands.OnEndpointSignedIn += h,
                    h => commands.OnEndpointSignedIn -= h)
                .Where(args => args.EventArgs.Endpoint.Equals(hostId))
                .Take(1);

            var availability = notificationAvailabilityNotifier.Subscribe(args => resetEvent.Set());
            using (availability)
            {
                if (!commands.HasCommandsFor(hostId))
                {
                    diagnostics.Log(
                        LevelToLog.Debug,
                        ExecutorConstants.LogPrefix,
                        Resources.Log_Messages_WaitingForHostToConnect);

                    resetEvent.WaitOne();
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching and then exiting the application with an error code, instead of letting WER handle it.")]
        private static TestExecutionResult ExecuteTest(EndpointId hostId, string storageDirectory, IContainer container)
        {
            var diagnostics = container.Resolve<SystemDiagnostics>();
            var notifications = container.Resolve<ITestExecutionNotificationsInvoker>();
            var fileSystem = container.Resolve<IFileSystem>();

            TestExecutionResult testResult;
            try
            {
                CreateStorageDirectory(fileSystem, storageDirectory, diagnostics);
            }
            catch (IOException e)
            {
                diagnostics.Log(
                    LevelToLog.Error,
                    ExecutorConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_FailedToCreateStorageDirectory_WithDirectoryAndError,
                        storageDirectory,
                        e));

                testResult = TestExecutionResult.Failed;
                notifications.RaiseOnTestCompletion(testResult);
                return testResult;
            }

            IEnumerable<TestStep> testSteps;
            IEnumerable<InputParameter> environmentParameters;
            try
            {
                DownloadTestData(hostId, storageDirectory, container, out testSteps, out environmentParameters);
            }
            catch (Exception)
            {
                testResult = TestExecutionResult.Failed;
                notifications.RaiseOnTestCompletion(testResult);
                return testResult;
            }

            var result = ExecuteTestSteps(container, testSteps, environmentParameters);
            diagnostics.Log(
                LevelToLog.Info,
                ExecutorConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TestCompleted_WithResult,
                    result));

            return result;
        }

        private static void CreateStorageDirectory(IFileSystem fileSystem, string storageDirectory, SystemDiagnostics diagnostics)
        {
            if (fileSystem.Directory.Exists(storageDirectory))
            {
                diagnostics.Log(
                    LevelToLog.Info,
                    ExecutorConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemovingStorageDirectory_WithDirectory,
                        storageDirectory));

                fileSystem.Directory.Delete(storageDirectory, true);
            }

            if (!fileSystem.Directory.Exists(storageDirectory))
            {
                diagnostics.Log(
                    LevelToLog.Debug,
                    ExecutorConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_CreatingStorageDirectory_WithDirectory,
                        storageDirectory));

                fileSystem.Directory.CreateDirectory(storageDirectory);
            }
        }

        private static void DownloadTestData(
            EndpointId hostId, 
            string storageDirectory, 
            IComponentContext container, 
            out IEnumerable<TestStep> testSteps,
            out IEnumerable<InputParameter> environmentParameters)
        {
            var diagnostics = container.Resolve<SystemDiagnostics>();
            var commandHub = container.Resolve<ISendCommandsToRemoteEndpoints>();
            var downloader = container.Resolve<DownloadDataFromRemoteEndpoints>();
            var fileSystem = container.Resolve<IFileSystem>();
            var packer = container.Resolve<ITestEnvironmentPackage>();

            diagnostics.Log(
                LevelToLog.Debug,
                ExecutorConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_DownloadingTestDataFromHost_WithHostUrl,
                    hostId));

            // Note that if this fails to get the command then it'll return null. Obviously the next method call will then
            // fail. That's ok, there's nothing we can do about it. Let the exception bubble up the stack and fix it there
            var transferCommands = commandHub.CommandsFor<ITransferTestDataCommands>(hostId);
            if (transferCommands == null)
            {
                throw new MissingCommandSetException();
            }

            var tokenTask = CommandSetGuard.GuardAgainstCommunicationFailure<Task<UploadToken>>(transferCommands.PrepareTestFilesForTransfer);
            var unzipTask = tokenTask.ContinueWith(
                t =>
                {
                    var file = Path.Combine(storageDirectory, fileSystem.Path.GetRandomFileName());

                    var streamTask = downloader(hostId, t.Result, file);
                    streamTask.Wait();

                    diagnostics.Log(
                        LevelToLog.Debug,
                        ExecutorConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_UnpackingTestData_WithSourceAndDestination,
                            streamTask.Result.FullName,
                            storageDirectory));
                    
                    var tempLocation = Path.Combine(storageDirectory, "environment");
                    packer.LoadAndUnpack(streamTask.Result.FullName, tempLocation);
                    foreach (var testPackage in packer.Tests())
                    {
                        testPackage.UnpackTo(Path.Combine(storageDirectory, testPackage.TestStepIndex.ToString()));
                    }
                });

            var testCaseTask = transferCommands.TestCase();
            var parametersTask = transferCommands.EnvironmentParameters();
            Task.WaitAll(unzipTask, testCaseTask, parametersTask);

            diagnostics.Log(
                LevelToLog.Debug,
                ExecutorConstants.LogPrefix,
                Resources.Log_Messages_TestDataTransferredSuccessfully);

            testSteps = testCaseTask.Result;
            environmentParameters = parametersTask.Result;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Just catching, logging and getting the hell out.")]
        private static TestExecutionResult ExecuteTestSteps(
            IContainer container, 
            IEnumerable<TestStep> testSteps,
            IEnumerable<InputParameter> environmentParameters)
        {
            var diagnostics = container.Resolve<SystemDiagnostics>();
            var notifications = container.Resolve<ITestExecutionNotificationsInvoker>();
            Action<string, TestSection> action = notifications.RaiseOnExecutionProgress;

            var sectionWriter = container.Resolve<ITestSectionBuilder>(
                new TypedParameter(typeof(string), Resources.ReportSection_Group_Name_TestExecution),
                new TypedParameter(typeof(Action<string, TestSection>), action));
            sectionWriter.Initialize(Resources.ReportSection_Name_TestExecution);
            var executors = container.Resolve<IEnumerable<IProcessTestStep>>().ToList();
            var executedSteps = new List<Tuple<TestStep, IParticipateInCleanUp>>();

            var testResult = TestExecutionResult.Passed;
            foreach (var step in testSteps)
            {
                try
                {
                    var executor = executors.Find(e => e.TestTypeToProcess == step.GetType());
                    if (executor == null)
                    {
                        diagnostics.Log(
                            LevelToLog.Error,
                            ExecutorConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_UnableToFindSuitableTestStepProcessor_WithTestStepType,
                                step.GetType()));

                        sectionWriter.AddErrorMessage(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Reporting_TestExecution_CouldNotFindSuitableExecutor_WithTestStepType,
                                step.GetType()));

                        sectionWriter.FinalizeAndStore(false);
                        notifications.RaiseOnTestCompletion(TestExecutionResult.Failed);
                        return TestExecutionResult.Failed;
                    }

                    diagnostics.Log(
                        LevelToLog.Debug,
                        ExecutorConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_ExecutingTestStep_WithTestStepType,
                            step.GetType()));

                    var result = executor.Process(step, environmentParameters);
                    var cleaner = executor as IParticipateInCleanUp;
                    if (cleaner != null)
                    {
                        executedSteps.Add(new Tuple<TestStep, IParticipateInCleanUp>(step, cleaner));
                    }

                    testResult = (result == TestExecutionState.Passed) ? TestExecutionResult.Passed : TestExecutionResult.Failed;
                    if (((result == TestExecutionState.Failed) || (result == TestExecutionState.Crashed)) 
                        && (step.FailureMode == TestStepFailureMode.Stop))
                    {
                        RollbackExecutedStepsOnFailure(executedSteps, diagnostics, sectionWriter);

                        sectionWriter.FinalizeAndStore(false);
                        notifications.RaiseOnTestCompletion(TestExecutionResult.Failed);
                        return TestExecutionResult.Failed;
                    }
                }
                catch (Exception e)
                {
                    diagnostics.Log(
                        LevelToLog.Error,
                        ExecutorConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_FailureWhileExecutingTest_WithError,
                            e));

                    sectionWriter.AddErrorMessage(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Reporting_TestExecution_Failed_WithException,
                            e));

                    if (step.FailureMode != TestStepFailureMode.Continue)
                    {
                        sectionWriter.FinalizeAndStore(false);
                        notifications.RaiseOnTestCompletion(TestExecutionResult.Failed);
                        return TestExecutionResult.Failed;
                    }
                }
            }

            sectionWriter.FinalizeAndStore(testResult == TestExecutionResult.Passed);
            notifications.RaiseOnTestCompletion(testResult);
            return testResult;
        }

        private static void RollbackExecutedStepsOnFailure(
            List<Tuple<TestStep, IParticipateInCleanUp>> executedSteps, 
            SystemDiagnostics diagnostics, 
            ITestSectionBuilder sectionWriter)
        {
            try
            {
                for (int i = executedSteps.Count - 1; i > -1; i--)
                {
                    var pair = executedSteps[i];
                    diagnostics.Log(
                        LevelToLog.Info,
                        ExecutorConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_CleaningUpTestStep_WithStepAndTestStepType,
                            pair.Item1.Order,
                            pair.Item1.GetType()));

                    pair.Item2.CleanUp(pair.Item1);
                }
            }
            catch (Exception e)
            {
                diagnostics.Log(
                    LevelToLog.Error,
                    ExecutorConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_FailedToCleanUp_WithError,
                        e));

                sectionWriter.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Reporting_TestExecution_CleanupFailed_WithException,
                        e));
            }
        }
    }
}
