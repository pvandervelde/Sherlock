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
using System.Management.Automation.Runspaces;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Executor.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    /// <summary>
    /// Processes an <see cref="TestStep"/> during which a script needs to be executed.
    /// </summary>
    internal sealed class ScriptExecuteTestStepProcessor : TestStepProcessor
    {
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
        /// Initializes a new instance of the <see cref="ScriptExecuteTestStepProcessor"/> class.
        /// </summary>
        /// <param name="testFileLocation">
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </param>
        /// <param name="reportFileUploader">
        /// The function that is used to upload the report files for the current test step.
        /// </param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
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
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionBuilder"/> is <see langword="null" />.
        /// </exception>
        public ScriptExecuteTestStepProcessor(
            RetrieveFileDataForTestStep testFileLocation,
            UploadReportFilesForTestStep reportFileUploader,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem,
            ITestSectionBuilder sectionBuilder)
            : base(testFileLocation, reportFileUploader, diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => sectionBuilder);
            }

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
                return typeof(ScriptExecuteTestStep);
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
                    test is ScriptExecuteTestStep,
                    Resources.Exceptions_Messages_InvalidTestStep);
            }

            var testStep = test as ScriptExecuteTestStep;
            var directory = TestFileLocationFor(testStep.Order);

            m_CurrentState = TestExecutionState.Running;
            m_SectionBuilder.Initialize("Script execution");
            try
            {
                try
                {
                    var logText = string.Format(
                        CultureInfo.InvariantCulture,
                        "Executing script of type: {0}",
                        testStep.ScriptLanguage);
                    Diagnostics.Log(
                        LevelToLog.Debug,
                        ScriptExecuteConstants.LogPrefix,
                        logText);
                    m_SectionBuilder.AddInformationMessage(logText);

                    switch (testStep.ScriptLanguage)
                    {
                        case ScriptLanguage.Powershell:
                            m_CurrentState = ProcessPowershellScript(testStep, environmentParameters, directory);
                            break;
                        default:
                            Diagnostics.Log(
                                LevelToLog.Error,
                                ScriptExecuteConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Unknown script language: {0}",
                                    testStep.ScriptLanguage));

                            m_SectionBuilder.AddErrorMessage("Unknown test script language.");
                            m_CurrentState = TestExecutionState.Failed;
                            break;
                    }
                }
                finally
                {
                    TransferReportFiles(m_SectionBuilder, testStep);
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Failed to run script. Error: {0}",
                    e);

                Diagnostics.Log(LevelToLog.Error, ScriptExecuteConstants.LogPrefix, errorMessage);
                m_SectionBuilder.AddErrorMessage(errorMessage);
                m_CurrentState = TestExecutionState.Failed;
            }
            finally
            {
                m_SectionBuilder.FinalizeAndStore(m_CurrentState == TestExecutionState.Passed);
            }

            return m_CurrentState;
        }

        private string GetScriptContents(string file)
        {
            return m_FileSystem.File.ReadAllText(file);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching because we don't know what to catch. Just want to exit directly after anyway.")]
        private TestExecutionState ProcessPowershellScript(
            ScriptExecuteTestStep testStep, 
            IEnumerable<InputParameter> environmentParameters, 
            string directory)
        {
            var result = TestExecutionState.Running;

            try
            {
                var installerFiles = m_FileSystem.Directory.GetFiles(directory, "*.ps1", SearchOption.TopDirectoryOnly);
                if (!installerFiles.Any())
                {
                    var noFilesFoundMessage = "No installer files found.";
                    Diagnostics.Log(LevelToLog.Error, ScriptExecuteConstants.LogPrefix, noFilesFoundMessage);
                    m_SectionBuilder.AddErrorMessage(noFilesFoundMessage);
                    result = TestExecutionState.Failed;
                }

                foreach (var scriptFile in installerFiles)
                {
                    var logText = string.Format(
                        CultureInfo.InvariantCulture,
                        "Loading powershell script from: {0}",
                        scriptFile);
                    Diagnostics.Log(
                        LevelToLog.Debug,
                        ScriptExecuteConstants.LogPrefix,
                        logText);
                    m_SectionBuilder.AddInformationMessage(logText);

                    var scriptContents = GetScriptContents(scriptFile);

                    // Create a new runspace for the script to execute in
                    var runSpace = RunspaceFactory.CreateRunspace();
                    runSpace.Open();
                    try
                    {
                        // Execute the script
                        var pipeline = runSpace.CreatePipeline();
                        var command = new Command(scriptContents, true);

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
                                "Adding parameter {0} with value {1}",
                                parameterSet.Key,
                                parameterSet.Value);
                            Diagnostics.Log(LevelToLog.Debug, ScriptExecuteConstants.LogPrefix, parameterInfo);
                            m_SectionBuilder.AddInformationMessage(parameterInfo);

                            command.Parameters.Add(parameterSet.Key, parameterSet.Value);
                        }

                        pipeline.Commands.Add(command);
                        try
                        {
                            var output = pipeline.Invoke();
                            foreach (var obj in output)
                            {
                                var text = obj.BaseObject.ToString();
                                if (!string.IsNullOrEmpty(text))
                                {
                                    var outputMessage = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "Powershell script output: {0}",
                                        text);
                                    Diagnostics.Log(LevelToLog.Info, ScriptExecuteConstants.LogPrefix, outputMessage);
                                    m_SectionBuilder.AddInformationMessage(outputMessage);
                                }
                            }

                            result = TestExecutionState.Passed;
                        }
                        catch (Exception e)
                        {
                            var errorMessage = string.Format(
                                CultureInfo.CurrentCulture,
                                "Failed to run script: {0}. Error: {1}",
                                scriptFile,
                                e);

                            Diagnostics.Log(LevelToLog.Error, ScriptExecuteConstants.LogPrefix, errorMessage);
                            m_SectionBuilder.AddErrorMessage(errorMessage);
                            result = TestExecutionState.Crashed;
                            break;
                        }
                    }
                    finally
                    {
                        runSpace.Close();
                    }

                    var informationMessage = string.Format(
                        CultureInfo.CurrentCulture,
                        "Ran script: {0}.",
                        scriptFile);
                    Diagnostics.Log(LevelToLog.Info, ScriptExecuteConstants.LogPrefix, informationMessage);
                    m_SectionBuilder.AddInformationMessage(informationMessage);
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "Failed to run script. Error: {0}",
                    e);

                Diagnostics.Log(LevelToLog.Error, ScriptExecuteConstants.LogPrefix, errorMessage);
                m_SectionBuilder.AddErrorMessage(errorMessage);
                result = TestExecutionState.Crashed;
            }

            return result;
        }
    }
}
