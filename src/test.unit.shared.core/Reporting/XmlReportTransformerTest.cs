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
using System.Linq;
using System.Reflection;
using System.Text;
using Nuclei;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
             Justification = "Unit tests do not need documentation.")]
    public sealed class XmlReportTransformerTest
    {
        private static IReport BuildReport(
           string productName,
           string productVersion,
           DateTimeOffset start,
           DateTimeOffset end,
           string sectionName,
           TestSection section)
        {
            var header = new ReportHeader(
               start,
               end,
               productName,
               productVersion);

            var testSections = new List<TestSection> { section };
            var reportSection = new ReportSection(sectionName, testSections);
            var sections = new List<ReportSection> { reportSection };

            return new Report(header, sections);
        }

        [Test]
        public void Transform()
        {
            var productName = "product";
            var productVersion = "1.2.3.4";
            var start = new DateTimeOffset(2000, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0));
            var end = start.AddSeconds(10);

            var name = "name";
            var section = new TestSection(
               "someName",
               start.AddSeconds(1),
               end.AddSeconds(-1),
               true,
               new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(2), "info") },
               new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(3), "warning") },
               new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(4), "error") });

            var report = BuildReport(productName, productVersion, start, end, name, section);

            Stream output = null;
            Action<string, Stream> writer =
               (fileName, input) =>
               {
                   output = input;
               };

            var transformer = new XmlReportTransformer();
            transformer.Transform(report, writer);

            var result = string.Empty;
            using (var reader = new StreamReader(output, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            var expected = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               @"Sherlock.Shared.Core.Reporting.TestXmlReport.xml");

            var builder = new StringBuilder(expected);
            {
                builder.Replace("${REPORT_VERSION}$", "1.0");
                builder.Replace("${HOST_NAME}$", Environment.MachineName);
                builder.Replace(
                   "${USER_NAME}$",
                   string.Format(
                      CultureInfo.InvariantCulture,
                      @"{0}\{1}",
                      Environment.UserDomainName,
                      Environment.UserName));
                builder.Replace("${SHERLOCK_VERSION}$", typeof(XmlReportTransformer).Assembly.GetName().Version.ToString());
                builder.Replace("${TEST_START_TIME}$", start.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${TEST_END_TIME}$", end.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${PRODUCT_NAME}$", productName);
                builder.Replace("${PRODUCT_VERSION}$", productVersion.ToString());
                builder.Replace("${REPORT_SECTION_NAME}$", name);
                builder.Replace("${TEST_SECTION_NAME}$", section.Name);
                builder.Replace("${TEST_SECTION_START_TIME}$", section.StartTime.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${TEST_SECTION_END_TIME}$", section.EndTime.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${TEST_SECTION_WAS_SUCCESSFUL}$", section.WasSuccessful.ToString());
                builder.Replace("${INFO_TIME}$", section.InfoMessages().First().Time.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${INFO_TEXT}$", section.InfoMessages().First().Information);
                builder.Replace("${WARNING_TIME}$", section.WarningMessages().First().Time.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${WARNING_TEXT}$", section.WarningMessages().First().Information);
                builder.Replace("${ERROR_TIME}$", section.ErrorMessages().First().Time.ToString("o", CultureInfo.CurrentCulture));
                builder.Replace("${ERROR_TEXT}$", section.ErrorMessages().First().Information);
            }

            Assert.AreEqual(
                builder.ToString().Replace("\r\n", string.Empty).Replace(" ", string.Empty),
                result.Replace("\r\n", string.Empty).Replace(" ", string.Empty));
        }
    }
}
