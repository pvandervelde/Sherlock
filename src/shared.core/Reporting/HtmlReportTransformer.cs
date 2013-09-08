//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Nuclei;

namespace Sherlock.Shared.Core.Reporting
{
    /// <summary>
    /// Transforms an <see cref="IReport"/> object into an HTML report.
    /// </summary>
    public sealed class HtmlReportTransformer : IReportTransformer
    {
        /// <summary>
        /// The name of the icon used to signify passing tests.
        /// </summary>
        private const string TestPassedIconName = "testspassed.png";

        /// <summary>
        /// The name of the icon used to signify failing tests.
        /// </summary>
        private const string TestFailedIconName = "testsfailed.png";

        /// <summary>
        /// The name of the icon used to signify an information message.
        /// </summary>
        private const string InfoMessageIconName = "info.png";

        /// <summary>
        /// The name of the icon used to signify a warning message.
        /// </summary>
        private const string WarningMessageIconName = "warning.png";

        /// <summary>
        /// The name of the icon used to signify an error message.
        /// </summary>
        private const string ErrorMessageIconName = "error.png";

        /// <summary>
        /// Creates the HTML that describes a report section.
        /// </summary>
        /// <param name="report">The report that contains the report section information.</param>
        /// <returns>A string containing the desired HTML.</returns>
        private static string CreateReportSectionHtml(IReport report)
        {
            var sortedSections = new List<Tuple<DateTimeOffset, string, TestSection>>();
            foreach (var reportSection in report.Sections())
            {
                foreach (var testSection in reportSection.Sections())
                {
                    sortedSections.Add(new Tuple<DateTimeOffset, string, TestSection>(testSection.StartTime, reportSection.Name, testSection));
                }
            }

            sortedSections.Sort((first, second) => first.Item1.CompareTo(second.Item1));
            var builder = new StringBuilder();
            {
                int section = 0;
                foreach (var pair in sortedSections)
                {
                    var sectionText = CreateTestSectionHtml(pair.Item2, pair.Item3, section % 2 == 0);
                    builder.AppendLine(sectionText);

                    section++;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates the HTML that describes a test section.
        /// </summary>
        /// <param name="category">The category of the section.</param>
        /// <param name="section">The report section that holds the test section information.</param>
        /// <param name="hasHighlight">A flag that indicates if this section should be highlighted.</param>
        /// <returns>A string containing the desired HTML.</returns>
        private static string CreateTestSectionHtml(string category, TestSection section, bool hasHighlight)
        {
            var testSectionTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               @"Sherlock.Shared.Core.Reporting.Templates.TestSectionTemplate.html");

            var messages = CreateTestSectionMessagesHtml(section);
            var builder = new StringBuilder(testSectionTemplate);
            {
                var highlightText = hasHighlight ? "class=\"row-light\"" : string.Empty;
                builder.Replace(@"${ROW_HIGHLIGHT}$", highlightText);
                
                builder.Replace(@"${SECTION_ICON}$", section.WasSuccessful ? TestPassedIconName : TestFailedIconName);
                builder.Replace(@"${SECTION_CATEGORY}$", category);
                builder.Replace(
                   @"${SECTION_DESCRIPTION}$",
                   HttpUtility.HtmlEncode(section.Name)
                      .ReplaceSpaceByHtmlNonBreakingSpaces()
                      .ReplaceTabsByHtmlNonBreakingSpaces()
                      .ReplaceNewLinesByHtmlLineBreaks());

                builder.Replace(@"${SECTION_MESSAGES}$", messages);
            }

            return builder.ToString();
        }

        private static string CreateTestSectionMessagesHtml(TestSection testSection)
        {
            var messages = new SortedList<DateTimeOffset, string>();

            AddTestSectionInfoMessages(messages, testSection);
            AddTestSectionWarningMessages(messages, testSection);
            AddTestSectionErrorMessages(messages, testSection);

            var builder = new StringBuilder();
            foreach (var msg in messages)
            {
                builder.AppendLine(msg.Value);
            }

            return builder.ToString();
        }

        private static void AddTestSectionInfoMessages(
           SortedList<DateTimeOffset, string> collectionToAddTo,
           TestSection testSection)
        {
            var infoMessageTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               @"Sherlock.Shared.Core.Reporting.Templates.TestSectionInfoTemplate.html");

            foreach (var info in testSection.InfoMessages())
            {
                var builder = new StringBuilder(infoMessageTemplate);
                {
                    builder.Replace(@"${INFO_ICON}$", InfoMessageIconName);
                    builder.Replace(@"${INFO_TIME}$", info.Time.ToString("G", CultureInfo.CurrentCulture));
                    builder.Replace(
                       @"${INFO_TEXT}$",
                       HttpUtility.HtmlEncode(info.Information)
                          .ReplaceNewLinesByHtmlLineBreaks()
                          .ReplaceTabsByHtmlNonBreakingSpaces());
                }

                AddMessageToTimeSortedCollection(collectionToAddTo, info.Time, builder.ToString());
            }
        }

        private static void AddTestSectionWarningMessages(
           SortedList<DateTimeOffset, string> collectionToAddTo,
           TestSection testSection)
        {
            var warningMessageTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               @"Sherlock.Shared.Core.Reporting.Templates.TestSectionWarningTemplate.html");

            foreach (var warning in testSection.WarningMessages())
            {
                var builder = new StringBuilder(warningMessageTemplate);
                {
                    builder.Replace(@"${WARNING_ICON}$", WarningMessageIconName);
                    builder.Replace(@"${WARNING_TIME}$", warning.Time.ToString("G", CultureInfo.CurrentCulture));
                    builder.Replace(
                       @"${WARNING_TEXT}$",
                       HttpUtility.HtmlEncode(warning.Information)
                          .ReplaceNewLinesByHtmlLineBreaks()
                          .ReplaceTabsByHtmlNonBreakingSpaces());
                }

                AddMessageToTimeSortedCollection(collectionToAddTo, warning.Time, builder.ToString());
            }
        }

        private static void AddTestSectionErrorMessages(
           SortedList<DateTimeOffset, string> collectionToAddTo,
           TestSection testSection)
        {
            var errorMessageTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               @"Sherlock.Shared.Core.Reporting.Templates.TestSectionErrorTemplate.html");

            foreach (var error in testSection.ErrorMessages())
            {
                var builder = new StringBuilder(errorMessageTemplate);
                {
                    builder.Replace(@"${ERROR_ICON}$", ErrorMessageIconName);
                    builder.Replace(@"${ERROR_TIME}$", error.Time.ToString("G", CultureInfo.CurrentCulture));
                    builder.Replace(
                       @"${ERROR_TEXT}$",
                       HttpUtility.HtmlEncode(error.Information)
                          .ReplaceNewLinesByHtmlLineBreaks()
                          .ReplaceTabsByHtmlNonBreakingSpaces());
                }

                AddMessageToTimeSortedCollection(collectionToAddTo, error.Time, builder.ToString());
            }
        }

        /// <summary>
        /// Adds the message to the time sorted collection performing
        /// some date/time manipulation if required.
        /// </summary>
        /// <param name="collectionToAddTo">The collection to which the messages should be added.</param>
        /// <param name="time">The time stamp of the message.</param>
        /// <param name="message">The message.</param>
        private static void AddMessageToTimeSortedCollection(
           SortedList<DateTimeOffset, string> collectionToAddTo,
           DateTimeOffset time,
           string message)
        {
            Debug.Assert(collectionToAddTo != null, "The collection should not be null.");

            // If the collection already has a message at the exact time then
            // we check if the messages match. If so then we just ignore this
            // message, otherwise we do something evil / cunning.
            if (collectionToAddTo.ContainsKey(time))
            {
                var storedMessage = collectionToAddTo[time];
                if (!string.Equals(message, storedMessage, StringComparison.Ordinal))
                {
                    // The messages are not the same, the time stamps however are.
                    // Seeing that it is unlikely that any program can generate
                    // more than one test message per (roughly) 8 nano-seconds
                    // we can assume that identical time stamps are due to the fact
                    // that the OS only has milisecond precision in the standard
                    // timers.
                    // The solution is then to simply add one 'tick' (i.e. a 100
                    // nano-second interval) to the time. It is expected that
                    // this trick will not cause massive shifts in the reported times
                    // for the last messages (i.e we'd need more than 10,000 messages
                    // to shift the last message by 1 second).
                    var newTime = time + new TimeSpan(1);
                    AddMessageToTimeSortedCollection(collectionToAddTo, newTime, message);
                }

                return;
            }

            collectionToAddTo.Add(time, message);
        }

        private static string CreateTestResultHtml(IReport report)
        {
            var hasPassed = true;
            foreach (var section in report.Sections().SelectMany(s => s.Sections()))
            {
                hasPassed = hasPassed && section.WasSuccessful;
                if (!hasPassed)
                {
                    break;
                }
            }

            var template = hasPassed
                ? @"Sherlock.Shared.Core.Reporting.Templates.ReportResultPassedTemplate.html"
                : @"Sherlock.Shared.Core.Reporting.Templates.ReportResultFailedTemplate.html";

            return EmbeddedResourceExtracter.LoadEmbeddedTextFile(Assembly.GetExecutingAssembly(), template);
        }

        /// <summary>
        /// Writes the embedded icon to disk.
        /// </summary>
        /// <param name="path">The path of the embedded icon.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileWriter">The file writer.</param>
        private static void WriteEmbeddedFileToDisk(
           string path,
           string fileName,
           Action<string, Stream> fileWriter)
        {
            var iconStream = EmbeddedResourceExtracter.LoadEmbeddedStream(
               Assembly.GetExecutingAssembly(),
               path);
            fileWriter(fileName, iconStream);
        }

        /// <summary>
        /// Gets the file extension for the transformed report.
        /// </summary>
        public string Extension
        {
            get
            {
                return ".html";
            }
        }

        /// <summary>
        /// Transforms the specified report into a pre-determined format and
        /// then stores the output in the stream.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="fileWriter">
        ///   The delegate that is used to write the given stream to a file
        ///   with the specified name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="report"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="fileWriter"/> is <see langword="null" />.
        /// </exception>
        public void Transform(IReport report, Action<string, Stream> fileWriter)
        {
            {
                Lokad.Enforce.Argument(() => report);
                Lokad.Enforce.Argument(() => fileWriter);
            }

            var reportSection = CreateReportSectionHtml(report);
            var testResult = CreateTestResultHtml(report);

            var reportTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               @"Sherlock.Shared.Core.Reporting.Templates.ReportTemplate.html");

            var builder = new StringBuilder(reportTemplate);
            {
                builder.Replace(
                   @"${REPORT_TITLE}$",
                   string.Format(
                      CultureInfo.CurrentCulture,
                      "Regression test report for {0} [{1}]",
                      report.Header.ProductName,
                      report.Header.ProductVersion));

                builder.Replace(@"${PRODUCT_NAME}$", report.Header.ProductName);
                builder.Replace(@"${PRODUCT_VERSION}$", report.Header.ProductVersion.ToString());

                // Write the time in the general date long time format
                builder.Replace(@"${TEST_START_DATE}$", report.Header.StartTime.ToString("d", CultureInfo.CurrentCulture));
                builder.Replace(@"${TEST_START_TIME}$", report.Header.StartTime.ToString("T", CultureInfo.CurrentCulture));
                builder.Replace(@"${TEST_END_DATE}$", report.Header.EndTime.ToString("d", CultureInfo.CurrentCulture));
                builder.Replace(@"${TEST_END_TIME}$", report.Header.EndTime.ToString("T", CultureInfo.CurrentCulture));
                builder.Replace(@"${TEST_TOTAL_TIME}$", (report.Header.EndTime - report.Header.StartTime).ToString("g", CultureInfo.CurrentCulture));

                builder.Replace(@"${SHERLOCK_VERSION}$", report.Header.SherlockVersion.ToString());
                builder.Replace(@"${HOST}$", report.Header.HostName);
                builder.Replace(@"${USERNAME}$", report.Header.UserName);

                builder.Replace(@"${REPORT_RESULT_TEXT}$", testResult);
                builder.Replace(@"${TEST_SECTIONS}$", reportSection);
            }

            // Write the data to an internal stream first. We do this because the
            // StreamWriter will close the stream when it gets disposed. When that happens
            // we won't be able to do anything with the output stream anymore (the stream
            // data will be flushed but that's no good if we're working with a memory stream).
            var resultStream = new MemoryStream();
            var internalStream = new MemoryStream();
            using (var writer = new StreamWriter(internalStream, Encoding.UTF8))
            {
                writer.Write(builder.ToString());
                writer.Flush();

                // Reset the stream position so that we can pump it.
                internalStream.Position = 0;
                internalStream.CopyTo(resultStream);
                resultStream.Position = 0;
            }

            fileWriter("sherlock.report.html", resultStream);

            // css file
            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.SherlockHtmlReport.css",
               "SherlockHtmlReport.css",
               fileWriter);

            // Icons
            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.favicon.ico",
               "favicon.ico",
               fileWriter);

            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.passed.png",
               TestPassedIconName,
               fileWriter);

            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.failed.png",
               TestFailedIconName,
               fileWriter);

            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.information.png",
               InfoMessageIconName,
               fileWriter);

            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.warning.png",
               WarningMessageIconName,
               fileWriter);

            WriteEmbeddedFileToDisk(
               @"Sherlock.Shared.Core.Reporting.Templates.error.png",
               ErrorMessageIconName,
               fileWriter);
        }
    }
}
