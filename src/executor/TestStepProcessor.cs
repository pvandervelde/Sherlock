//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Nuclei.Diagnostics;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    /// <summary>
    /// Defines the base class for types that process test steps.
    /// </summary>
    internal abstract class TestStepProcessor : IProcessTestStep
    {
        /// <summary>
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </summary>
        private readonly RetrieveFileDataForTestStep m_TestFileLocation;

        /// <summary>
        /// The function that is used to upload the report files for the current test step to the host.
        /// </summary>
        private readonly UploadReportFilesForTestStep m_ReportFileUploader;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepProcessor"/> class.
        /// </summary>
        /// <param name="testFileLocation">
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </param>
        /// <param name="reportFileUploader">
        /// The function that is used to upload the report files for the current test step.
        /// </param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testFileLocation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reportFileUploader"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected TestStepProcessor(
            RetrieveFileDataForTestStep testFileLocation, 
            UploadReportFilesForTestStep reportFileUploader, 
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => testFileLocation);
                Lokad.Enforce.Argument(() => reportFileUploader);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_TestFileLocation = testFileLocation;
            m_ReportFileUploader = reportFileUploader;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets the object that provides the diagnostics methods for the application.
        /// </summary>
        protected SystemDiagnostics Diagnostics
        {
            get
            {
                return m_Diagnostics;
            }
        }

        /// <summary>
        /// Returns the full path to the directory which contains the files required by the current test step.
        /// </summary>
        /// <param name="stepIndex">The name of the test step.</param>
        /// <returns>The full path to the directory that contains the files for the current test step.</returns>
        protected string TestFileLocationFor(int stepIndex)
        {
            return m_TestFileLocation(stepIndex);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of test step that can be processed.
        /// </summary>
        public abstract Type TestTypeToProcess
        {
            get;
        }

        /// <summary>
        /// Processes the given test step.
        /// </summary>
        /// <param name="test">The test step that should be processed.</param>
        /// <param name="environmentParameters">The collection that provides the parameters for the environment.</param>
        /// <returns>The state of the test after the test step has been executed.</returns>
        public abstract TestExecutionState Process(TestStep test, IEnumerable<InputParameter> environmentParameters);

        /// <summary>
        /// Transfers the report files back to the host.
        /// </summary>
        /// <param name="sectionBuilder">The object that stores the report messages.</param>
        /// <param name="testStep">The test step for which the files should be uploaded.</param>
        /// <param name="additionalFilesToInclude">A collection containing the additional files that should be included in the report.</param>
        protected void TransferReportFiles(
            ITestSectionBuilder sectionBuilder,
            TestStep testStep,
            IEnumerable<string> additionalFilesToInclude = null)
        {
            try
            {
                var filesToTransfer = new Dictionary<FileInfo, DirectoryInfo>();
                foreach (var file in testStep.ReportFiles)
                {
                    if (File.Exists(file.Path))
                    {
                        filesToTransfer.Add(new FileInfo(file.Path), new DirectoryInfo(Path.GetDirectoryName(file.Path)));
                    }
                }

                foreach (var directory in testStep.ReportDirectories)
                {
                    if (Directory.Exists(directory.Path))
                    {
                        var files = Directory.GetFiles(directory.Path, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            filesToTransfer.Add(new FileInfo(file), new DirectoryInfo(directory.Path));
                        }
                    }
                }

                if (additionalFilesToInclude != null)
                {
                    foreach (var file in additionalFilesToInclude)
                    {
                        if (File.Exists(file))
                        {
                            filesToTransfer.Add(new FileInfo(file), new DirectoryInfo(Path.GetDirectoryName(file)));
                        }
                    }
                }
        
                if (testStep.ReportIncludesSystemLog)
                {
                    var logFile = ConsoleExecuteConstants.LogPath();
                    if (File.Exists(logFile))
                    {
                        filesToTransfer.Add(new FileInfo(logFile), new DirectoryInfo(Path.GetDirectoryName(logFile)));
                    }
                }

                m_ReportFileUploader(testStep.Order, filesToTransfer);

                sectionBuilder.AddInformationMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Uploaded report files for test step: {0}",
                        testStep.Order));
            }
            catch (Exception e)
            {
                sectionBuilder.AddWarningMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to upload report files for test step: {0}. Error was: {1}",
                        testStep.Order,
                        e));
            }
        }
    }
}
