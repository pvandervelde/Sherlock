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
using System.Reflection;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Handles the execution of tests.
    /// </summary>
    internal sealed class TestController : IControlTests
    {
        private static string CreateLocalStorageDirectory()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            Debug.Assert(attributes.Length == 1, "There should only be 1 AssemblyProductAttribute");

            var product = ((AssemblyProductAttribute)attributes[0]).Product;
            var path = Path.Combine(
                Path.GetTempPath(),
                product,
                Guid.NewGuid().ToString("D"));

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (IOException)
                {
                }
            }

            return path;
        }

        private static TestStep CopyStepAndStripNonEssentialInformation(TestStep step)
        {
            var parameters = new List<TestStepParameter>();
            foreach (var parameter in step.Parameters)
            {
                parameters.Add(
                    new TestStepParameter
                    {
                        Key = parameter.Key,
                        Value = parameter.Value,
                    });
            }

            var reportDirectories = new List<TestStepReportDirectory>();
            foreach (var directory in step.ReportDirectories)
            {
                reportDirectories.Add(
                    new TestStepReportDirectory
                    {
                        Path = directory.Path,
                    });
            }

            var reportFiles = new List<TestStepReportFile>();
            foreach (var file in step.ReportFiles)
            {
                reportFiles.Add(
                    new TestStepReportFile
                    {
                        Path = file.Path,
                    });
            }

            TestStep result = null;
            var consoleStep = step as ConsoleExecuteTestStep;
            if (consoleStep != null)
            {
                var newStep = new ConsoleExecuteTestStep
                {
                    Order = step.Order,
                    FailureMode = step.FailureMode,
                    ReportIncludesSystemLog = step.ReportIncludesSystemLog,
                    ExecutableFilePath = consoleStep.ExecutableFilePath,
                };

                result = newStep;
            }

            var msiStep = step as MsiInstallTestStep;
            if (msiStep != null)
            {
                var newStep = new MsiInstallTestStep
                {
                    Order = step.Order,
                    FailureMode = step.FailureMode,
                    ReportIncludesSystemLog = step.ReportIncludesSystemLog,
                };

                result = newStep;
            }

            var scriptStep = step as ScriptExecuteTestStep;
            if (scriptStep != null)
            {
                var newStep = new ScriptExecuteTestStep
                {
                    Order = step.Order,
                    FailureMode = step.FailureMode,
                    ReportIncludesSystemLog = step.ReportIncludesSystemLog,
                    ScriptLanguage = scriptStep.ScriptLanguage,
                };

                result = newStep;
            }

            var xcopyStep = step as XCopyTestStep;
            if (xcopyStep != null)
            {
                var newStep = new XCopyTestStep
                {
                    Order = step.Order,
                    FailureMode = step.FailureMode,
                    ReportIncludesSystemLog = step.ReportIncludesSystemLog,
                    Destination = xcopyStep.Destination,
                };

                result = newStep;
            }

            result.Parameters = parameters;
            result.ReportDirectories = reportDirectories;
            result.ReportFiles = reportFiles;
            return result;
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that stores all the activators.
        /// </summary>
        /// <remarks>
        /// It is expected that there will only be a handful activators. Hence using a list
        /// is just as good as using a dictionary.
        /// </remarks>
        private readonly List<IEnvironmentActivator> m_Activators;

        /// <summary>
        /// The object that provides access to the configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The object that stores the tests that are currently being executed.
        /// </summary>
        private readonly IStoreActiveTests m_ExecutingTests;

        /// <summary>
        /// The object that stores all the known test information.
        /// </summary>
        private readonly Func<IProvideTestingContext> m_TestContextFactory;

        /// <summary>
        /// The object provides test suite packages.
        /// </summary>
        private readonly Func<ITestSuitePackage> m_TestSuitePackageFactory;

        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The function that is used to create a report builder.
        /// </summary>
        private readonly Func<IReportBuilder> m_ReportBuilders;

        /// <summary>
        /// The function that is used to create a section builder.
        /// </summary>
        private readonly Func<string, IReportBuilder, ITestSectionBuilder> m_SectionBuilders;

        /// <summary>
        /// The object that provides diagnostics methods.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The directory in which test data is unpacked.
        /// </summary>
        private readonly string m_UnpackDirectory = CreateLocalStorageDirectory();

        /// <summary>
        /// A flag that indicates if the controller is in the process of activating one or more tests.
        /// </summary>
        private volatile bool m_IsActivatingTests;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="executingTests">The object that stores the tests that are currently being executed.</param>
        /// <param name="testContextFactory">The object that creates a context which provides access to all test information.</param>
        /// <param name="activators">The collection of machine activators.</param>
        /// <param name="testSuitePackageFactory">The object that handles packing and unpacking test data.</param>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="reportBuilders">The function that is used to create a report.</param>
        /// <param name="sectionBuilders">The function that is used to create a test report section.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executingTests"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testContextFactory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="activators"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testSuitePackageFactory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reportBuilders"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionBuilders"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TestController(
            IConfiguration configuration,
            IStoreActiveTests executingTests,
            Func<IProvideTestingContext> testContextFactory,
            IEnumerable<IEnvironmentActivator> activators,
            Func<ITestSuitePackage> testSuitePackageFactory,
            IFileSystem fileSystem,
            Func<IReportBuilder> reportBuilders,
            Func<string, IReportBuilder, ITestSectionBuilder> sectionBuilders,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => testContextFactory);
                Lokad.Enforce.Argument(() => activators);
                Lokad.Enforce.Argument(() => testSuitePackageFactory);
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => reportBuilders);
                Lokad.Enforce.Argument(() => sectionBuilders);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Configuration = configuration;
            m_TestContextFactory = testContextFactory;
            m_Activators = new List<IEnvironmentActivator>(activators);
            m_TestSuitePackageFactory = testSuitePackageFactory;
            m_FileSystem = fileSystem;
            m_ReportBuilders = reportBuilders;
            m_SectionBuilders = sectionBuilders;
            m_Diagnostics = diagnostics;

            m_ExecutingTests = executingTests;
            m_ExecutingTests.OnTestCompletion += HandleTestCompleted;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Realistically we don't care what happens we want it to complete normally.")]
        private void HandleTestCompleted(object sender, TestExecutionResultEventArgs eventArgs)
        {
            lock (m_Lock)
            {
                var currentContext = m_TestContextFactory();

                var shouldTerminateEnvironment = m_Configuration.HasValueFor(MasterServiceConfigurationKeys.ShouldTerminateEnvironmentOnFailedTest)
                    ? m_Configuration.Value<bool>(MasterServiceConfigurationKeys.ShouldTerminateEnvironmentOnFailedTest)
                    : MasterServiceConstants.DefaultShouldTerminateEnvironmentOnFailedTest;

                if (shouldTerminateEnvironment || (eventArgs.Result == TestExecutionResult.Passed))
                {
                    foreach (var environment in m_ExecutingTests.EnvironmentsForTest(eventArgs.Id))
                    {
                        try
                        {
                            environment.Shutdown();
                            currentContext.MarkMachineAsInactive(environment.Environment);
                        }
                        catch (Exception e)
                        {
                            m_Diagnostics.Log(
                                LevelToLog.Error,
                                MasterServiceConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Failed to shut down the machine {0}. Error was: {1}",
                                    environment.Environment,
                                    e));
                        }
                    }
                }

                var report = m_ExecutingTests.ReportFor(eventArgs.Id);
                var notifications = m_ExecutingTests.NotificationsFor(eventArgs.Id);
                foreach (var notification in notifications)
                {
                    notification.OnTestCompleted(eventArgs.Result, report);
                }

                m_ExecutingTests.Remove(eventArgs.Id);
                currentContext.StopTest(eventArgs.Id);
                try
                {
                    var testFile = TestHelpers.StoragePathForTestFiles(eventArgs.Id, m_Configuration, m_FileSystem);
                    if (m_FileSystem.File.Exists(testFile))
                    {
                        m_FileSystem.File.Delete(testFile);
                    }
                }
                catch (IOException e)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        MasterServiceConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_FailedToDeleteTestFiles_WithFileAndError,
                            eventArgs.Id,
                            e));
                }

                m_Diagnostics.Log(
                    LevelToLog.Info,
                    MasterServiceConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_CompletedTest_WithId,
                        eventArgs.Id));
            }
        }

        /// <summary>
        /// Activates all the tests for which environments can be found.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Realistically we don't care what happens we want it to complete normally.")]
        public void ActivateTests()
        {
            if (m_IsActivatingTests)
            {
                return;
            }

            lock (m_Lock)
            {
                m_IsActivatingTests = true;
                try
                {
                    var currentContext = m_TestContextFactory();
                    foreach (var test in currentContext.InactiveTests())
                    {
                        var environments = test.Environments;
                        if (!environments.Any())
                        {
                            m_Diagnostics.Log(
                                LevelToLog.Info,
                                MasterServiceConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_NoEnvironmentForTestCase,
                                    test.Id));
                            continue;
                        }

                        var matchingEnvironments = SelectEnvironments(environments);
                        if ((matchingEnvironments == null) || (matchingEnvironments.Count < environments.Count()))
                        {
                            m_Diagnostics.Log(
                                LevelToLog.Info,
                                MasterServiceConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_NoEnvironmentForTestCase,
                                    test.Id));
                            continue;
                        }

                        var builder = m_ReportBuilders();
                        builder.InitializeNewReport(
                            test.ProductName,
                            test.ProductVersion);
                    
                        // @todo: Should really do the creation of the CompletedNotification via the IOC container
                        m_ExecutingTests.Add(
                            test.Id, 
                            builder,
                            new List<TestCompletedNotification>
                                {
                                    new FileBasedTestCompletedNotification(test.ReportPath),
                                });

                        LoadEnvironmentsAndStartTest(currentContext, test, matchingEnvironments, builder);
                    }
                }
                finally
                {
                    m_IsActivatingTests = false;
                }
            }
        }

        private void LoadEnvironmentsAndStartTest(
            IProvideTestingContext currentContext, 
            Test test, 
            List<Tuple<MachineDescription, TestEnvironment>> matchingEnvironments, 
            IReportBuilder builder)
        {
            var initializeSection = m_SectionBuilders(Resources.ReportSection_Group_Name_Initialization, builder);
            initializeSection.Initialize(Resources.ReportSection_Name_LoadEnvironments);
            var parameters = matchingEnvironments.Select(t => new InputParameter(t.Item2.Name, t.Item1.NetworkName));
            var testFile = TestHelpers.StoragePathForTestFiles(test.Id, m_Configuration, m_FileSystem);
            try
            {
                try
                {
                    foreach (var pair in matchingEnvironments)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Debug,
                            MasterServiceConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_ActivatingEnvironment_WithTestAndEnvironment,
                                test.Id,
                                pair.Item2.Name));

                        var environmentSection = m_SectionBuilders(Resources.ReportSection_Group_Name_Environments, builder);
                        environmentSection.Initialize(pair.Item2.Name);
                        var environment = ActivateMachineForEnvironment(currentContext, pair.Item1, pair.Item2, environmentSection);
                        m_ExecutingTests.AddEnvironmentForTest(test.Id, environment);

                        var testSteps = new List<TestStep>();
                        foreach (var step in currentContext.TestStepsForEnvironment(pair.Item2.Id).OrderBy(s => s.Order))
                        {
                            // Copy the test steps and strip all links that are not remotely related to any of the work
                            // we're about to do.
                            var copy = CopyStepAndStripNonEssentialInformation(step);
                            testSteps.Add(copy);
                        }

                        m_Diagnostics.Log(
                            LevelToLog.Debug,
                            MasterServiceConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_PackagingTestFilesForEnvironment_WithTestAndEnvironment,
                                test.Id,
                                pair.Item2));

                        var suite = m_TestSuitePackageFactory();
                        suite.LoadAndUnpack(testFile, Path.Combine(m_UnpackDirectory, test.Id.ToString(CultureInfo.InvariantCulture), pair.Item2.Name));
                        var environmentFile = suite.Environment(pair.Item2.Name);

                        m_Diagnostics.Log(
                            LevelToLog.Debug,
                            MasterServiceConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_ExecutingTestOnEnvironment_WithTestAndEnvironment,
                                test.Id,
                                pair.Item2));
                        environment.Execute(test.Id, testSteps, parameters, environmentFile.PackagePath);
                    }

                    // Start the test
                    currentContext.StartTest(test.Id);
                }
                finally
                {
                    var logText = string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_StartedTestOnEnvironments,
                        test.Id,
                        string.Join(", ", matchingEnvironments.Select(t => new Tuple<string, string>(t.Item1.Id, t.Item2.Name))));
                    m_Diagnostics.Log(LevelToLog.Info, MasterServiceConstants.LogPrefix, logText);
                }
            }
            catch (Exception e)
            {
                // complete the test
                var activeEnvironments = m_ExecutingTests.EnvironmentsForTest(test.Id).ToList();
                foreach (var environment in activeEnvironments)
                {
                    try
                    {
                        environment.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error,
                            MasterServiceConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_FailedToShutdownEnvironment_WithError,
                                ex));
                    }
                }

                m_ExecutingTests.Remove(test.Id);
                if (test.StartTime.HasValue && !test.EndTime.HasValue)
                {
                    currentContext.StopTest(test.Id);
                }

                // In all cases do we remove the test. If it falls over then a report will be produced showing
                // why it didn't work
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    MasterServiceConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_FailedToStartEnvironments_WithError,
                        e));
                initializeSection.AddErrorMessage(Resources.ReportSection_Error_FailedToStartEnvironments);
                initializeSection.FinalizeAndStore(false);
            }
        }

        /// <summary>
        /// Select the environments on which a test case can be executed.
        /// </summary>
        /// <param name="environments">The collection of environments placed on the selection of the machine.</param>
        /// <returns>
        ///     A collection containing a mapping between the most suitable environments and the environments
        ///     that machine matched against.
        /// </returns>
        private List<Tuple<MachineDescription, TestEnvironment>> SelectEnvironments(IEnumerable<TestEnvironment> environments)
        {
            var result = new List<Tuple<MachineDescription, TestEnvironment>>();

            var context = m_TestContextFactory();
            foreach (var environment in environments)
            {
                var suitableEnvironments = context.InactiveMachinesWith(
                    environment.OperatingSystem,
                    environment.Applications);

                // Select the most suitable machine. Ideally we would base that on:
                // * The given test will load the machine nearly to the maximum, but doesn't overload it to
                //   provide optimal usage of resources
                //   * The indication of busy should should probably be based on:
                //     * If the total usage of memory, disk or CPU is close to the maximum (be it through
                //       one process or multiple processes) then the machine is busy.
                //     * In case of hosted machines (i.e. virtual machines) if the host machine is busy
                //       then all the hosted machine(s) attached to this host machine should be considered
                //       busy (because a new VM won't be able to get enough CPU / RAM).
                // * Depending on the type of test we want to run it may also matter if an interactive
                //   user is logged on to the machine. For instance for GUI tests we can't have a user
                //   interactively using the machine.
                // * Minimal set of applications available. Again for optimal use of resources
                //
                // The general idea should be to leave the more specialistic machines for the more demanding
                // tests.
                //
                // It is probably a better idea to pass on a list of suitable machines and then 
                // check for busy-ness when we are about to connect to a machine. That way we 
                // minimize the window between selection and usage.
                // Also there may be times were we want to wait for a machine to become available
                // instead of grabbing a machine that is 'over-qualified'.
                //
                // For now we just select the first machine
                var selectedEnvironment = suitableEnvironments.FirstOrDefault();
                if (selectedEnvironment != null)
                {
                    result.Add(new Tuple<MachineDescription, TestEnvironment>(selectedEnvironment, environment));
                }
            }

            return result;
        }

        /// <summary>
        /// Activates the given machine and returns an object that can be used to manipulate
        /// the machine.
        /// </summary>
        /// <param name="currentContext">The current data storage context.</param>
        /// <param name="machine">The description of the machine.</param>
        /// <param name="environment">The description of the environment.</param>
        /// <param name="sectionBuilder">
        ///     The object used to write information to the report about the starting and stopping of the machine.
        /// </param>
        /// <returns>An object that is used to manipulate the active machine.</returns>
        private IActiveEnvironment ActivateMachineForEnvironment(
            IProvideTestingContext currentContext, 
            MachineDescription machine, 
            TestEnvironment environment, 
            ITestSectionBuilder sectionBuilder)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => machine);
                    Lokad.Enforce.With<ArgumentException>(
                        m_Activators.Any(a => a.EnvironmentTypeToLoad == machine.GetType()),
                        Resources.Exceptions_Messages_NoActivatorHasBeenRegisteredForTheEnvironment);
                }

                var activator = m_Activators.Find(a => a.EnvironmentTypeToLoad == machine.GetType());
                Debug.Assert(activator != null, "We should have found an activator.");

                var activeEnvironment = activator.Load(machine, sectionBuilder, OnEnvironmentUnloaded);

                currentContext.MarkMachineAsActive(machine.Id);
                currentContext.TestEnvironmentSupportedByMachine(environment.Id, machine.Id);

                return activeEnvironment;
            }
        }

        private void OnEnvironmentUnloaded(string id)
        {
            lock (m_Lock)
            {
                m_TestContextFactory().MarkMachineAsInactive(id);
            }
        }
    }
}
