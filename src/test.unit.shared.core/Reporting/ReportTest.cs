//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ReportTest
    {
        [Test]
        public void RoundTripSerialise()
        {
            var productName = "product";
            var productVersion = "1.2.3.4";
            var owner = "owner";
            var description = "description";
            var start = DateTimeOffset.Now;
            var end = start.AddSeconds(10);

            var header = new ReportHeader(
                start,
                end,
                productName,
                productVersion,
                owner,
                description);

            var name = "name";
            var section = new TestSection(
                "someName",
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                true,
                new List<DateBasedTestInformation>(),
                new List<DateBasedTestInformation>(),
                new List<DateBasedTestInformation>());
            var testSections = new List<TestSection>
                {
                    section
                };
            var reportSection = new ReportSection(name, testSections);
            var sections = new List<ReportSection>
                {
                    reportSection
                };

            var report = new Report(header, sections);
            var otherReport = AssertExtensions.RoundTripSerialize(report);

            // We'll assume that if one of the fields is correct then the other fields will be correct
            // too. This is (relatively) safe because there is a serialize / deserialize test for
            // the ReportHeader class.
            Assert.AreEqual(report.Header.ProductName, otherReport.Header.ProductName);

            // We'll assume that if one of the fields is correct then the other fields will be correct
            // too. This is (relatively) safe because there is a serialize / deserialize test for
            // the TestSection class.
            Assert.AreEqual(report.Sections().Count(), otherReport.Sections().Count());
            Assert.AreEqual(report.Sections().First().Name, otherReport.Sections().First().Name);
        }

        [Test]
        public void Create()
        {
            var productName = "product";
            var productVersion = "1.2.3.4";
            var owner = "owner";
            var description = "description";
            var start = DateTimeOffset.Now;
            var end = start.AddSeconds(10);

            var header = new ReportHeader(
                start,
                end,
                productName,
                productVersion,
                owner,
                description);

            var name = "name";
            var section = new TestSection(
                "someName",
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                true,
                new List<DateBasedTestInformation>(),
                new List<DateBasedTestInformation>(),
                new List<DateBasedTestInformation>());
            var testSections = new List<TestSection>
                {
                    section
                };
            var reportSection = new ReportSection(name, testSections);
            var sections = new List<ReportSection>
                {
                    reportSection
                };

            var report = new Report(header, sections);

            Assert.AreSame(header, report.Header);
            Assert.That(
                report.Sections(),
                Is.EquivalentTo(sections));
        }
    }
}
