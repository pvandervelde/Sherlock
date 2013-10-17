//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Processes an <see cref="TestStep"/> during which a console application needs to be executed.
    /// </summary>
    internal sealed class ConsoleExecuteTestStepProcessor : TestStepProcessor
    {
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
        /// Initializes a new instance of the <see cref="ConsoleExecuteTestStepProcessor"/> class.
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
        public ConsoleExecuteTestStepProcessor(
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
                return typeof(ConsoleExecuteTestStep);
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
        ///     Thrown if <paramref name="test"/> is not a <see cref="ScriptExecuteTestStep"/>.
        /// </exception>
        public override TestExecutionState Process(TestStep test, IEnumerable<InputParameter> environmentParameters)
        {
            {
                Lokad.Enforce.Argument(() => test);
                Lokad.Enforce.With<ArgumentException>(
                    test is ConsoleExecuteTestStep,
                    Resources.Exceptions_Messages_InvalidTestStep);
            }

            var testStep = test as ConsoleExecuteTestStep;

            m_CurrentState = TestExecutionState.Running;
            m_SectionBuilder.Initialize("Application execution");
            try
            {
                try
                {
                    var parameters = testStep.Parameters
                        .OrderBy(p => int.Parse(p.Key, CultureInfo.InvariantCulture))
                        .Select(p => p.Value)
                        .ToList();
                    int returnCode = RunApplication(testStep.ExecutableFilePath, parameters);
                    if (returnCode != 0)
                    {
                        var errorMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            "Failed to execute {0} {1}.",
                            m_FileSystem.Path.GetFileName(testStep.ExecutableFilePath),
                            string.Join(" ", parameters));
                        Diagnostics.Log(LevelToLog.Error, ConsoleExecuteConstants.LogPrefix, errorMessage);
                        m_SectionBuilder.AddErrorMessage(errorMessage);

                        m_CurrentState = TestExecutionState.Failed;
                    }
                    else
                    {
                        var informationMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            "Executed: {0} {1}.",
                            m_FileSystem.Path.GetFileName(testStep.ExecutableFilePath),
                            string.Join(" ", parameters));
                        Diagnostics.Log(LevelToLog.Info, ConsoleExecuteConstants.LogPrefix, informationMessage);
                        m_SectionBuilder.AddInformationMessage(informationMessage);

                        m_CurrentState = TestExecutionState.Passed;
                    }
                }
                finally
                {
                    TransferReportFiles(m_SectionBuilder, testStep);
                }
            }
            finally
            {
                m_SectionBuilder.FinalizeAndStore(m_CurrentState == TestExecutionState.Passed);
            }

            return m_CurrentState;
        }

        private int RunApplication(string executablePath, List<string> arguments)
        {
            int returnCode;
            EventHandler<ProcessOutputEventArgs> handler =
                (s, e) =>
                    Diagnostics.Log(
                        LevelToLog.Trace,
                        MsiDeployConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0} output: {1}",
                            m_FileSystem.Path.GetFileNameWithoutExtension(executablePath),
                            e.Output));
            try
            {
                m_Runner.OnConsoleOutput += handler;
                returnCode = m_Runner.Run(executablePath, arguments.ToArray());
            }
            catch (Exception e)
            {
                var errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Failed to execute {0} {1}. Error: {2}",
                    m_FileSystem.Path.GetFileName(executablePath),
                    string.Join(" ", arguments),
                    e);

                Diagnostics.Log(LevelToLog.Error, ConsoleExecuteConstants.LogPrefix, errorMessage);
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
