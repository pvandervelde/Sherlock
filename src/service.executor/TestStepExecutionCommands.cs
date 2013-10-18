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
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nuclei;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Executor.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Defines the methods used to execute test cases.
    /// </summary>
    internal sealed class TestStepExecutionCommands : IExecuteTestStepsCommands
    {
        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The object that provides methods for sending and receiving data from remote endpoints.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that sends commands to remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_RemoteCommands;

        /// <summary>
        /// The object that provides notifications from remote endpoints.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_RemoteNotifications;

        /// <summary>
        /// The function that handles the download of data from a remote endpoint.
        /// </summary>
        private readonly DownloadDataFromRemoteEndpoints m_DataDownload;

        /// <summary>
        /// The object that handles sending test execution events to the Sherlock master.
        /// </summary>
        private readonly ITestExecutionNotificationsInvoker m_TestExecutionEvents;

        /// <summary>
        /// The function that returns a new test section builder.
        /// </summary>
        private readonly Func<string, ITestSectionBuilder> m_SectionBuilders;

        /// <summary>
        /// The object that stores information about the currently active test.
        /// </summary>
        private readonly ActiveTestInformation m_TestInformation;

        /// <summary>
        /// The configuration object.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The object that stores information about the host.
        /// </summary>
        private readonly HostInformationStorage m_HostInformation;

        /// <summary>
        /// The directory in which all data files are stored.
        /// </summary>
        private readonly string m_StorageDirectory;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The endpoint ID of the current executor application.
        /// </summary>
        private EndpointId m_CurrentEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepExecutionCommands"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="layer">The object that provides methods for sending and receiving data from remote endpoints.</param>
        /// <param name="remoteCommands">The object that sends commands to remote endpoints.</param>
        /// <param name="remoteNotifications">The object that provides notifications from remote endpoints.</param>
        /// <param name="dataDownload">The function that is used to download data streams from remote endpoints.</param>
        /// <param name="executionEvents">The object that handles sending test execution events to the Sherlock master.</param>
        /// <param name="sectionBuilders">The function that returns a new test section builder.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="testInformation">The object that stores information about the currently active test.</param>
        /// <param name="hostInformation">The object that stores information about the host.</param>
        /// <param name="storageDirectory">The directory in which all the report files are stored.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="remoteCommands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="remoteNotifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataDownload"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executionEvents"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionBuilders"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hostInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageDirectory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TestStepExecutionCommands(
            IFileSystem fileSystem,
            ICommunicationLayer layer,
            ISendCommandsToRemoteEndpoints remoteCommands,
            INotifyOfRemoteEndpointEvents remoteNotifications,
            DownloadDataFromRemoteEndpoints dataDownload,
            ITestExecutionNotificationsInvoker executionEvents,
            Func<string, ITestSectionBuilder> sectionBuilders,
            IConfiguration configuration,
            ActiveTestInformation testInformation,
            HostInformationStorage hostInformation,
            string storageDirectory,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => layer);
                Lokad.Enforce.Argument(() => remoteCommands);
                Lokad.Enforce.Argument(() => remoteNotifications);
                Lokad.Enforce.Argument(() => dataDownload);
                Lokad.Enforce.Argument(() => executionEvents);
                Lokad.Enforce.Argument(() => sectionBuilders);
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => testInformation);
                Lokad.Enforce.Argument(() => hostInformation);
                Lokad.Enforce.Argument(() => storageDirectory);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_FileSystem = fileSystem;
            m_Layer = layer;
            m_RemoteCommands = remoteCommands;
            m_RemoteNotifications = remoteNotifications;
            m_DataDownload = dataDownload;
            m_TestExecutionEvents = executionEvents;
            m_SectionBuilders = sectionBuilders;
            m_Configuration = configuration;
            m_TestInformation = testInformation;
            m_HostInformation = hostInformation;
            m_StorageDirectory = storageDirectory;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Starts the execution of the given test steps.
        /// </summary>
        /// <param name="testSteps">The collection of test steps that should be executed.</param>
        /// <param name="environmentParameters">The collection that contains the parameters related to the test environment.</param>
        /// <param name="callingEndpoint">The ID of the endpoint that called the method.</param>
        /// <param name="token">The upload token that can be used to upload the file.</param>
        /// <returns>A task which completes when the execution has started successfully.</returns>
        public Task Execute(
            List<TestStep> testSteps,
            List<InputParameter> environmentParameters,
            EndpointId callingEndpoint,
            UploadToken token)
        {
            return Task.Factory.StartNew(() => StartNewTestExecution(testSteps, environmentParameters, callingEndpoint, token));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching to send back a failure message and then exit the test.")]
        private void StartNewTestExecution(
            List<TestStep> testSteps,
            List<InputParameter> environmentParameters,
            EndpointId callingEndpoint, 
            UploadToken token)
        {
            var builder = m_SectionBuilders(Resources.ReportSection_Group_Name_Initialization);
            builder.Initialize(Resources.ReportSection_Name_Initialization);

            m_HostInformation.Id = callingEndpoint;

            string testFile;
            if (!DownloadTestData(callingEndpoint, token, m_StorageDirectory, builder, out testFile))
            {
                return;
            }

            m_TestInformation.TestSteps = testSteps;
            m_TestInformation.TestPackage = testFile;
            m_TestInformation.EnvironmentParameters = environmentParameters;

            try
            {
                var pair = m_Layer.LocalConnectionFor(ChannelType.NamedPipe);
                m_CurrentEndpoint = StartExecutorApplication(pair.Item1, pair.Item2);

                m_TestInformation.CurrentState = TestExecutionState.Running;
                m_Diagnostics.Log(
                    LevelToLog.Debug,
                    ExecutorServiceConstants.LogPrefix,
                    Resources.Log_Messages_StartedTestApplication);
            }
            catch (Exception e)
            {
                m_TestInformation.CurrentState = TestExecutionState.None;

                m_Diagnostics.Log(
                    LevelToLog.Error,
                    ExecutorServiceConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_FailedToStartTestApplication_WithError,
                        e));

                builder.AddErrorMessage(Resources.ReportSection_Error_FailedToStartTestingApplication);
                builder.FinalizeAndStore(false);
                m_TestExecutionEvents.RaiseOnTestCompletion(TestExecutionResult.Failed);
                return;
            }

            builder.AddInformationMessage(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.ReportSection_Info_StartedTestApplication_WithId,
                    m_CurrentEndpoint));

            try
            {
                WaitForExecutorConnection(m_CurrentEndpoint);
            }
            catch (TestExecutionFailureException)
            {
                m_TestInformation.CurrentState = TestExecutionState.None;

                m_Diagnostics.Log(
                    LevelToLog.Error,
                    ExecutorServiceConstants.LogPrefix,
                    Resources.Log_Messages_FailedToConnectToTestApplication_WithError);

                builder.AddErrorMessage(Resources.ReportSection_Error_FailedToConnectToTestingApplication);
                builder.FinalizeAndStore(false);
                m_TestExecutionEvents.RaiseOnTestCompletion(TestExecutionResult.Failed);
                return;
            }

            ConnectEvents(m_CurrentEndpoint);
            builder.FinalizeAndStore(true);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching because we don't actually know what can be thrown.")]
        private bool DownloadTestData(
            EndpointId callingEndpoint,
            UploadToken token,
            string storageDirectory,
            ITestSectionBuilder builder,
            out string testFile)
        {
            testFile = string.Empty;
            try
            {
                testFile = DownloadTestData(callingEndpoint, token, storageDirectory);

                m_Diagnostics.Log(
                    LevelToLog.Debug,
                    ExecutorServiceConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_DownloadedTestData_WithFile,
                        testFile));
                builder.AddInformationMessage(Resources.ReportSection_Info_DownloadedTestData);
            }
            catch (Exception)
            {
                builder.AddErrorMessage(Resources.ReportSection_Error_FailedToDownloadTestData);
                builder.FinalizeAndStore(false);
                m_TestExecutionEvents.RaiseOnTestCompletion(TestExecutionResult.Failed);

                return false;
            }

            return true;
        }

        private string DownloadTestData(EndpointId callingEndpoint, UploadToken token, string storageDirectory)
        {
            var filePath = m_FileSystem.Path.Combine(
                storageDirectory,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.zip",
                    Guid.NewGuid().ToString("D")));
            var fileStream = m_DataDownload(callingEndpoint, token, filePath);
            fileStream.Wait();

            return fileStream.Result.FullName;
        }

        private EndpointId StartExecutorApplication(EndpointId endpointId, Uri address)
        {
            var localInstallDir = Assembly.GetExecutingAssembly().LocalDirectoryPath();
            var fullFilePath = Path.Combine(localInstallDir, "Sherlock.Executor.exe");
            var arguments = string.Format(
                CultureInfo.InvariantCulture,
                @"--host={0} --channeltype=""{1}"" --channeluri={2}",
                endpointId,
                ChannelType.NamedPipe,
                address);

            var startInfo = new ProcessStartInfo
                {
                    FileName = fullFilePath,
                    Arguments = arguments,

                    // Create no window for the process. It doesn't need one
                    // and we don't want the user to have a window they can 
                    // kill. If the process has to die then the user will have
                    // to put in some effort.
                    CreateNoWindow = true,

                    // Do not use the shell to create the application
                    // This means we can only start executables.
                    UseShellExecute = false,

                    // Do not display an error dialog if startup fails
                    // We'll deal with that ourselves
                    ErrorDialog = false,

                    // Do not redirect any of the input or output streams
                    // We don't really care about them because we won't be using
                    // them anyway.
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,

                    // Set the working directory to something sane. Mostly the
                    // directory of the file that we're trying to read.
                    WorkingDirectory = localInstallDir,
                };

            var exec = new Process
                {
                    StartInfo = startInfo
                };

            EventHandler handler = null;
            handler =
                (s, e) =>
                {
                    if ((m_TestInformation.CurrentState == TestExecutionState.Running)
                        || m_TestInformation.CurrentState == TestExecutionState.Unknown)
                    {
                        // The process must have crashed
                        m_TestExecutionEvents.RaiseOnTestCompletion(TestExecutionResult.Failed);
                    }

                    m_TestInformation.CurrentState = TestExecutionState.None;
                    exec.Exited -= handler;
                };
            exec.Exited += handler;

            exec.Start();
            return exec.CreateEndpointIdForProcess();
        }

        private void WaitForExecutorConnection(EndpointId endpoint)
        {
            var resetEvent = new AutoResetEvent(false);
            var commandAvailabilityNotifier =
                        Observable.FromEventPattern<CommandSetAvailabilityEventArgs>(
                            h => m_RemoteCommands.OnEndpointSignedIn += h,
                            h => m_RemoteCommands.OnEndpointSignedIn -= h)
                        .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                        .Take(1);

            var notificationAvailabilityNotifier =
                Observable.FromEventPattern<NotificationSetAvailabilityEventArgs>(
                    h => m_RemoteNotifications.OnEndpointSignedIn += h,
                    h => m_RemoteNotifications.OnEndpointSignedIn -= h)
                .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                .Take(1);

            var availability = commandAvailabilityNotifier
                .Zip(notificationAvailabilityNotifier, (a, b) => true)
                .Subscribe(args => resetEvent.Set());
            
            using (availability)
            {
                if (!m_RemoteNotifications.HasNotificationsFor(endpoint))
                {
                    var timeoutInMilliSeconds = m_Configuration.HasValueFor(ExecutorServiceConfigurationKeys.ExecutorStartupTimeoutInMilliSeconds)
                        ? m_Configuration.Value<int>(ExecutorServiceConfigurationKeys.ExecutorStartupTimeoutInMilliSeconds)
                        : ExecutorServiceConstants.DefaultExecutorStartupTimeoutInMilliSeconds;

                    if (!resetEvent.WaitOne(timeoutInMilliSeconds))
                    {
                        throw new TestExecutionFailureException();
                    }
                }
            }
        }

        private void ConnectEvents(EndpointId endpoint)
        {
            var notifications = m_RemoteNotifications.NotificationsFor<ITestExecutionNotifications>(endpoint);

            EventHandler<TestExecutionProgressEventArgs> progressHandle =
                (s, e) => m_TestExecutionEvents.RaiseOnExecutionProgress(e.SectionName, e.Section);
            notifications.OnExecutionProgress += progressHandle;

            EventHandler<TestExecutionResultEventArgs> completionHandle = null;
            completionHandle =
                (s, e) =>
                {
                    notifications.OnExecutionProgress -= progressHandle;
                    notifications.OnTestCompletion -= completionHandle;

                    m_TestInformation.CurrentState = TestExecutionState.None;
                    m_TestExecutionEvents.RaiseOnTestCompletion(e.Result);
                    m_CurrentEndpoint = null;
                };
            notifications.OnTestCompletion += completionHandle;
        }

        /// <summary>
        /// Returns a value indicating the current state of the test.
        /// </summary>
        /// <returns>A task that returns the current state of the test.</returns>
        public Task<TestExecutionState> State()
        {
            return Task<TestExecutionState>.Factory.StartNew(() => m_TestInformation.CurrentState);
        }

        /// <summary>
        /// Stops the execution of the currently running test.
        /// </summary>
        /// <returns>A task that completes when the stop process is finished.</returns>
        public Task Terminate()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var endpoint = m_CurrentEndpoint;
                    if (endpoint == null)
                    {
                        return;
                    }

                    var commands = m_RemoteCommands.CommandsFor<IExecutorCommands>(endpoint);
                    if (commands != null)
                    {
                        var task = commands.Terminate();
                        try
                        {
                            task.Wait();
                        }
                        catch (AggregateException)
                        {
                        }
                    }
                });
        }
    }
}
