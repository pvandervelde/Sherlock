//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Executor.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    /// <summary>
    /// Processes an <see cref="TestStep"/> during which a MSI file needs to be installed on the test environment.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Msi",
        Justification = "It's the MSI system we're dealing with here.")]
    internal sealed class MsiDeployTestStepProcessor : TestStepProcessor
    {
        /// <summary>
        /// The name of the msiexec application.
        /// </summary>
        private const string MsiInstallerExeName = "msiexec.exe";

        /// <summary>
        /// The switch used by msiexec to install an application.
        /// </summary>
        private const string InstallSwitch = @"/i ""{0}"" ";

        /// <summary>
        /// The switch used by msiexec to log the install process.
        /// </summary>
        /// <remarks>
        /// The following command line arguments are available for msiexec:
        /// i:   Logs status messages
        /// w:   Logs nonfatal warnings
        /// e:   Logs all error messages
        /// a:   Logs startup of actions
        /// r:   Logs action-specific records
        /// u:   Logs user requests
        /// c:   Logs initial interface parameters
        /// m:   Logs out of memory
        /// p:   Logs terminal properties
        /// v:   Logs verbose output.
        /// </remarks>
        /// <source>
        /// http://www.microsoft.com/resources/documentation/windows/xp/all/proddocs/en-us/msiexec.mspx?mfr=true
        /// </source>
        private const string LogSwitch = @"/Lime! ""{0}"" ";

        /// <summary>
        /// The switch used by msiexec to hide the user interface.
        /// </summary>
        private const string QuietSwitch = @" /qn ";

        /// <summary>
        /// The switch used by msiexec to set parameters on the command line.
        /// </summary>
        private const string ParameterSwitch = @"{0}={1}";

        /// <summary>
        /// The console app runner.
        /// </summary>
        private readonly IRunConsoleApplications m_Runner;

        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The test section builder that is used to create the test section for the
        /// report.
        /// </summary>
        private readonly ITestSectionBuilder m_SectionBuilder;

        /// <summary>
        /// The current state of the test execution.
        /// </summary>
        private TestExecutionState m_CurrentState = TestExecutionState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsiDeployTestStepProcessor"/> class.
        /// </summary>
        /// <param name="testFileLocation">
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </param>
        /// <param name="reportFileUploader">
        /// The function that is used to upload the report files for the current test step.
        /// </param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="runner">The object that is used to execute console applications.</param>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="sectionBuilder">The section builder.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testFileLocation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reportFileUploader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="runner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionBuilder"/> is <see langword="null" />.
        /// </exception>
        public MsiDeployTestStepProcessor(
            RetrieveFileDataForTestStep testFileLocation,
            UploadReportFilesForTestStep reportFileUploader,
            SystemDiagnostics diagnostics,
            IRunConsoleApplications runner,
            IFileSystem fileSystem,
            ITestSectionBuilder sectionBuilder)
            : base(testFileLocation, reportFileUploader, diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => runner);
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => sectionBuilder);
            }

            m_Runner = runner;
            m_FileSystem = fileSystem;
            m_SectionBuilder = sectionBuilder;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of test step that can be processed.
        /// </summary>
        public override Type TestTypeToProcess
        {
            get
            {
                return typeof(MsiInstallTestStep);
            }
        }

        /// <summary>
        /// Processes the given test step.
        /// </summary>
        /// <param name="test">The test step that should be processed.</param>
        /// <param name="environmentParameters">The collection that provides the parameters for the environment.</param>
        /// <returns>The state of the test after the test step has been executed.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="test"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="test"/> is not a <see cref="MsiInstallTestStep"/>.
        /// </exception>
        public override TestExecutionState Process(TestStep test, IEnumerable<InputParameter> environmentParameters)
        {
            {
                Lokad.Enforce.Argument(() => test);
                Lokad.Enforce.With<ArgumentException>(
                    test is MsiInstallTestStep,
                    Resources.Exceptions_Messages_InvalidTestStep);
            }

            var testStep = test as MsiInstallTestStep;
            var directory = TestFileLocationFor(testStep.Order);
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            m_CurrentState = TestExecutionState.Running;
            m_SectionBuilder.Initialize("MSI install");
            try
            {
                var logFiles = new List<string>();
                try
                {
                    var installerFiles = m_FileSystem.Directory.GetFiles(directory, "*.msi", SearchOption.TopDirectoryOnly);
                    if (!installerFiles.Any())
                    {
                        const string noFilesFoundMessage = "No installer files found.";
                        Diagnostics.Log(LevelToLog.Error, ScriptExecuteConstants.LogPrefix, noFilesFoundMessage);
                        m_SectionBuilder.AddErrorMessage(noFilesFoundMessage);
                        m_CurrentState = TestExecutionState.Failed;
                    }

                    foreach (var installerFile in installerFiles)
                    {
                        // Log everything to the default log file.
                        var logFile = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(installerFile) + ".log");
                        logFiles.Add(logFile);
                        var arguments = new List<string> 
                            { 
                                QuietSwitch,
                                string.Format(CultureInfo.InvariantCulture, LogSwitch, logFile),
                                string.Format(CultureInfo.InvariantCulture, InstallSwitch, installerFile),
                            };

                        var environmentTestStepParameters = environmentParameters.Select(
                            e => new TestStepParameter
                                {
                                    Key = e.Key, 
                                    Value = e.Value
                                });

                        foreach (var parameterSet in testStep.Parameters.Concat(environmentTestStepParameters))
                        {
                            var parameterInfo = string.Format(
                                CultureInfo.InvariantCulture,
                                "Adding installer parameter {0} with value {1}",
                                parameterSet.Key,
                                parameterSet.Value);
                            Diagnostics.Log(LevelToLog.Debug, MsiDeployConstants.LogPrefix, parameterInfo);
                            m_SectionBuilder.AddInformationMessage(parameterInfo);

                            arguments.Add(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    ParameterSwitch,
                                    parameterSet.Key,
                                    parameterSet.Value));
                        }

                        int returnCode = RunInstaller(arguments, installerFile);
                        if (returnCode != 0)
                        {
                            var errorMessage = string.Format(
                                CultureInfo.CurrentCulture,
                                "Failed to install {0}.",
                                installerFile);
                            Diagnostics.Log(LevelToLog.Error, MsiDeployConstants.LogPrefix, errorMessage);
                            m_SectionBuilder.AddErrorMessage(errorMessage);

                            m_CurrentState = TestExecutionState.Failed;
                        }
                        else
                        {
                            var informationMessage = string.Format(
                                CultureInfo.CurrentCulture,
                                "Installed: {0}.",
                                installerFile);
                            Diagnostics.Log(LevelToLog.Info, MsiDeployConstants.LogPrefix, informationMessage);
                            m_SectionBuilder.AddInformationMessage(informationMessage);

                            m_CurrentState = TestExecutionState.Passed;
                        }
                    }
                }
                finally
                {
                    TransferReportFiles(
                        m_SectionBuilder, 
                        testStep, 
                        testStep.ReportIncludesSystemLog ? logFiles : null);
                }
            }
            finally
            {
                m_SectionBuilder.FinalizeAndStore(m_CurrentState == TestExecutionState.Passed);
            }

            return m_CurrentState;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Not sure what to catch here. Just want to catch and get out.")]
        private int RunInstaller(List<string> arguments, string installer)
        {
            int returnCode;
            EventHandler<ProcessOutputEventArgs> handler = 
                (s, e) => 
                    Diagnostics.Log(
                        LevelToLog.Trace,
                        MsiDeployConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "MsiExec output: {0}",
                            e.Output));
            try
            {
                m_Runner.OnConsoleOutput += handler;
                returnCode = m_Runner.Run(MsiInstallerExeName, arguments.ToArray());
            }
            catch (Exception e)
            {
                var errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Failed to install {0}. Error: {1}",
                    installer,
                    e);

                Diagnostics.Log(LevelToLog.Error, MsiDeployConstants.LogPrefix, errorMessage);
                m_SectionBuilder.AddErrorMessage(errorMessage);
                returnCode = -1;
            }
            finally
            {
                m_Runner.OnConsoleOutput -= handler;
            }

            return returnCode;
        }
    }
}
