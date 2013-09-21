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
using System.Xml.Linq;
using Nuclei;

namespace Sherlock.Shared.Core.Reporting
{
    /// <summary>
    /// Transforms an <see cref="IReport"/> object into an XML report.
    /// </summary>
    public sealed class XmlReportTransformer : IReportTransformer
    {
        private static string EscapeTextForUseInXml(string input)
        {
            var element = new XElement("FakeElement", input);
            return element.FirstNode.ToString();
        }

        private static string DetermineTestPassOrFail(IReport report)
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

            return hasPassed ? @"Passed" : @"Failed";
        }

        /// <summary>
        /// Creates the XML that describes a report section.
        /// </summary>
        /// <param name="report">The report that contains the report section information.</param>
        /// <returns>A string containing the desired XML.</returns>
        private static string CreateReportSectionXml(IReport report)
        {
            var reportSectionTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                @"Sherlock.Shared.Core.Reporting.Templates.ReportSectionTemplate.xml");
            string reportSections = string.Empty;

            foreach (var reportSection in report.Sections())
            {
                var testSections = CreateTestSectionXml(reportSection);
                var builder = new StringBuilder(reportSectionTemplate);
                {
                    builder.Replace(@"${REPORT_SECTION_NAME}$", EscapeTextForUseInXml(reportSection.Name));
                    builder.Replace(@"${TEST_SECTIONS}$", testSections);
                }

                reportSections += builder.ToString();
            }

            return reportSections;
        }

        /// <summary>
        /// Creates the XML that describes a test section.
        /// </summary>
        /// <param name="section">The report section that holds the test section information.</param>
        /// <returns>
        /// A string containing the desired XML.
        /// </returns>
        private static string CreateTestSectionXml(ReportSection section)
        {
            var testSectionTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                @"Sherlock.Shared.Core.Reporting.Templates.TestSectionTemplate.xml");
            string testSections = string.Empty;

            foreach (var testSection in section.Sections())
            {
                var messages = CreateTestSectionMessagesXml(testSection);
                var builder = new StringBuilder(testSectionTemplate);
                {
                    builder.Replace(@"${TEST_SECTION_NAME}$", EscapeTextForUseInXml(testSection.Name));
                    builder.Replace(
                        @"${TEST_SECTION_START_TIME}$",
                        EscapeTextForUseInXml(testSection.StartTime.ToString("o", CultureInfo.CurrentCulture)));
                    builder.Replace(
                        @"${TEST_SECTION_END_TIME}$",
                        EscapeTextForUseInXml(testSection.EndTime.ToString("o", CultureInfo.CurrentCulture)));
                    builder.Replace(
                        @"${TEST_SECTION_WAS_SUCCESSFUL}$",
                        EscapeTextForUseInXml(testSection.WasSuccessful.ToString()));

                    builder.Replace(@"${TEST_SECTION_MESSAGES}$", messages);
                }

                testSections += builder.ToString();
            }

            return testSections;
        }

        private static string CreateTestSectionMessagesXml(TestSection testSection)
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
                @"Sherlock.Shared.Core.Reporting.Templates.TestSectionInfoTemplate.xml");

            foreach (var info in testSection.InfoMessages())
            {
                var builder = new StringBuilder(infoMessageTemplate);
                {
                    builder.Replace(
                        @"${INFO_TIME}$",
                        EscapeTextForUseInXml(info.Time.ToString("o", CultureInfo.CurrentCulture)));
                    builder.Replace(@"${INFO_TEXT}$", EscapeTextForUseInXml(info.Information));
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
                @"Sherlock.Shared.Core.Reporting.Templates.TestSectionWarningTemplate.xml");

            foreach (var info in testSection.WarningMessages())
            {
                var builder = new StringBuilder(warningMessageTemplate);
                {
                    builder.Replace(
                        @"${WARNING_TIME}$",
                        EscapeTextForUseInXml(info.Time.ToString("o", CultureInfo.CurrentCulture)));
                    builder.Replace(@"${WARNING_TEXT}$", EscapeTextForUseInXml(info.Information));
                }

                AddMessageToTimeSortedCollection(collectionToAddTo, info.Time, builder.ToString());
            }
        }

        private static void AddTestSectionErrorMessages(
            SortedList<DateTimeOffset, string> collectionToAddTo,
            TestSection testSection)
        {
            var errorMessageTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                @"Sherlock.Shared.Core.Reporting.Templates.TestSectionErrorTemplate.xml");

            foreach (var info in testSection.ErrorMessages())
            {
                var builder = new StringBuilder(errorMessageTemplate);
                {
                    builder.Replace(
                        @"${ERROR_TIME}$",
                        EscapeTextForUseInXml(info.Time.ToString("o", CultureInfo.CurrentCulture)));
                    builder.Replace(@"${ERROR_TEXT}$", EscapeTextForUseInXml(info.Information));
                }

                AddMessageToTimeSortedCollection(collectionToAddTo, info.Time, builder.ToString());
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

        /// <summary>
        /// Gets the file extension for the transformed report.
        /// </summary>
        public string Extension
        {
            get
            {
                return ".xml";
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

            var reportSection = CreateReportSectionXml(report);

            var reportTemplate = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                @"Sherlock.Shared.Core.Reporting.Templates.ReportTemplate.xml");

            var builder = new StringBuilder(reportTemplate);
            {
                builder.Replace(@"${REPORT_VERSION}$", "1.0");
                builder.Replace(@"${HOST_NAME}$", EscapeTextForUseInXml(report.Header.HostName));
                builder.Replace(@"${USER_NAME}$", EscapeTextForUseInXml(report.Header.UserName));
                builder.Replace(
                    @"${SHERLOCK_VERSION}$",
                    EscapeTextForUseInXml(report.Header.SherlockVersion.ToString()));

                // Write the time in a round-trip fashion. Specify the 'o' formatter for this
                builder.Replace(
                    @"${TEST_START_TIME}$",
                    EscapeTextForUseInXml(report.Header.StartTime.ToString("o", CultureInfo.CurrentCulture)));
                builder.Replace(
                    @"${TEST_END_TIME}$",
                    EscapeTextForUseInXml(report.Header.EndTime.ToString("o", CultureInfo.CurrentCulture)));

                builder.Replace(@"${PRODUCT_NAME}$", EscapeTextForUseInXml(report.Header.ProductName));
                builder.Replace(@"${PRODUCT_VERSION}$", EscapeTextForUseInXml(report.Header.ProductVersion.ToString()));

                builder.Replace(@"${TEST_RESULT}$", DetermineTestPassOrFail(report));

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

            fileWriter("sherlock.report.xml", resultStream);
        }
    }
}
