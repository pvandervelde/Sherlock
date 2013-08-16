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
        /// Sends out a notification indicating that the test has completed.
        /// </summary>
        /// <param name="result">The test result.</param>
        /// <param name="report">The report describing the results of the test.</param>
        public override void OnTestCompleted(TestExecutionResult result, IReport report)
        {
            var transformer = new HtmlReportTransformer();
            transformer.Transform(report, CopyStream);
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
