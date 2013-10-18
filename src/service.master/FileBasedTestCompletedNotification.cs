//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Notifies the user that a test has been completed by generating one or more files in a given directory.
    /// </summary>
    [Serializable]
    internal sealed class FileBasedTestCompletedNotification : TestCompletedNotification
    {
        /// <summary>
        /// The full path to the directory where the files should be dropped.
        /// </summary>
        private readonly string m_OutputPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedTestCompletedNotification"/> class.
        /// </summary>
        /// <param name="outputDirectory">The full path to the directory that where the files should be dropped.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="outputDirectory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="outputDirectory"/> is an empty string.
        /// </exception>
        public FileBasedTestCompletedNotification(string outputDirectory)
        {
            {
                Lokad.Enforce.Argument(() => outputDirectory);
                Lokad.Enforce.Argument(() => outputDirectory, Lokad.Rules.StringIs.NotEmpty);
            }

            m_OutputPath = outputDirectory;
        }

        /// <summary>
        /// Stores the given file in the location where the test report will be stored.
        /// </summary>
        /// <param name="filePath">The full path to the file that should be included in the report.</param>
        /// <param name="relativeReportPath">The relative 'path' which should be used to store the file data.</param>
        public override void StoreReportFile(string filePath, string relativeReportPath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var destinationPath = Path.Combine(m_OutputPath, relativeReportPath);
                    File.Copy(filePath, destinationPath);
                }
            }
            catch (IOException)
            {
                // Just ignore it for now.
            }
        }

        /// <summary>
        /// Sends out a notification indicating that the test has completed.
        /// </summary>
        /// <param name="result">The test result.</param>
        /// <param name="report">The report describing the results of the test.</param>
        public override void OnTestCompleted(TestExecutionResult result, IReport report)
        {
            try
            {
                var htmlTransformer = new HtmlReportTransformer();
                htmlTransformer.Transform(report, CopyStream);
            }
            catch (Exception)
            {
                // Just continue with processing
            }

            try
            {
                var xmlTransformer = new XmlReportTransformer();
                xmlTransformer.Transform(report, CopyStream);
            }
            catch (Exception)
            {
                // Just continue with processing
            }
        }

        private void CopyStream(string fileName, Stream input)
        {
            var path = Path.Combine(m_OutputPath, fileName);
            using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                input.CopyTo(outputStream);
            }
        }
    }
}
