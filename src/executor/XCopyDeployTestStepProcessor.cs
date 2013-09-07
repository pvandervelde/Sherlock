//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Executor.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    /// <summary>
    /// Processes an <see cref="TestStep"/> during which one or more files and directories need to be copied to the
    /// test environment.
    /// </summary>
    internal sealed class XCopyDeployTestStepProcessor : TestStepProcessor
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
        /// Initializes a new instance of the <see cref="XCopyDeployTestStepProcessor"/> class.
        /// </summary>
        /// <param name="testFileLocation">
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="sectionBuilder">The section builder.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testFileLocation"/> is <see langword="null" />.
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
        public XCopyDeployTestStepProcessor(
            RetrieveFileDataForTestStep testFileLocation,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem,
            ITestSectionBuilder sectionBuilder) 
            : base(testFileLocation, diagnostics)
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
                return typeof(XCopyTestStep);
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
        ///     Thrown if <paramref name="test"/> is not a <see cref="XCopyTestStep"/>.
        /// </exception>
        public override TestExecutionState Process(TestStep test, IEnumerable<InputParameter> environmentParameters)
        {
            {
                Lokad.Enforce.Argument(() => test);
                Lokad.Enforce.With<ArgumentException>(
                    test is XCopyTestStep,
                    Resources.Exceptions_Messages_InvalidTestStep);
            }

            var testStep = test as XCopyTestStep;
            if (!m_FileSystem.Directory.Exists(testStep.Destination))
            {
                try
                {
                    m_FileSystem.Directory.CreateDirectory(testStep.Destination);
                }
                catch (IOException)
                {
                    Diagnostics.Log(
                        LevelToLog.Error,
                        XCopyDeployConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to create test directory at location: {0}",
                            testStep.Destination));

                    return TestExecutionState.Failed;
                }
            }

            var directory = TestFileLocationFor(testStep.Order);

            m_CurrentState = TestExecutionState.Running;
            m_SectionBuilder.Initialize("X-copy installer.");
            try
            {
                var installerFiles = m_FileSystem.Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                foreach (var installerFile in installerFiles)
                {
                    var relativePath = installerFile.Substring(directory.Length).TrimStart(Path.DirectorySeparatorChar);
                    var destination = Path.Combine(testStep.Destination, relativePath);
                    try
                    {
                        var copyInformation = string.Format(
                            CultureInfo.InvariantCulture,
                            "Copying from: {0}; To: {1}",
                            installerFile,
                            destination);
                        Diagnostics.Log(
                            LevelToLog.Debug,
                            XCopyDeployConstants.LogPrefix,
                            copyInformation);
                        m_SectionBuilder.AddInformationMessage(copyInformation);

                        m_FileSystem.File.Copy(installerFile, destination);
                    }
                    catch (IOException e)
                    {
                        var errorMessage = string.Format(
                           CultureInfo.CurrentCulture,
                           "Failed to install {0}. Error: {1}",
                           installerFile,
                           e);

                        Diagnostics.Log(LevelToLog.Error, XCopyDeployConstants.LogPrefix, errorMessage);
                        m_SectionBuilder.AddErrorMessage(errorMessage);

                        m_CurrentState = TestExecutionState.Failed;
                        break;
                    }

                    var informationMessage = string.Format(
                             CultureInfo.CurrentCulture,
                             "Installed: {0}.",
                             installerFile);
                    Diagnostics.Log(LevelToLog.Info, XCopyDeployConstants.LogPrefix, informationMessage);
                    m_SectionBuilder.AddInformationMessage(informationMessage);
                }

                m_CurrentState = TestExecutionState.Passed;
            }
            finally
            {
                m_SectionBuilder.FinalizeAndStore(m_CurrentState == TestExecutionState.Passed);
            }

            return m_CurrentState;
        }
    }
}
