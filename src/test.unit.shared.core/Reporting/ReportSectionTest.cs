//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Core.Testing;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
             Justification = "Unit tests do not need documentation.")]
    public sealed class ReportSectionTest
    {
        [Test]
        public void RoundTripSerialise()
        {
            var name = "name";
            var section = new TestSection(
               "someName",
               DateTimeOffset.Now,
               DateTimeOffset.Now,
               true,
               new List<DateBasedTestInformation>(),
               new List<DateBasedTestInformation>(),
               new List<DateBasedTestInformation>());
            var sections = new List<TestSection> { section };

            var reportSection = new ReportSection(name, sections);
            var otherReportSection = AssertExtensions.RoundTripSerialize(reportSection);

            Assert.AreEqual(reportSection.Name, otherReportSection.Name);

            // We'll assume that if one of the fields is correct then the other fields will be correct
            // too. This is (relatively) safe because there is a serialize / deserialize test for
            // the TestSection class.
            Assert.AreEqual(reportSection.Sections().Count(), otherReportSection.Sections().Count());
            Assert.AreEqual(reportSection.Sections().First().Name, otherReportSection.Sections().First().Name);
        }

        [Test]
        public void CreateWithEmptySectionName()
        {
            Assert.Throws<ArgumentException>(() => new ReportSection(string.Empty, new List<TestSection>()));
        }

        [Test]
        public void Create()
        {
            var name = "name";
            var section = new TestSection(
               "someName",
               DateTimeOffset.Now,
               DateTimeOffset.Now,
               true,
               new List<DateBasedTestInformation>(),
               new List<DateBasedTestInformation>(),
               new List<DateBasedTestInformation>());
            var sections = new List<TestSection> { section };

            var reportSection = new ReportSection(name, sections);

            Assert.AreEqual(name, reportSection.Name);
            Assert.That(
                reportSection.Sections(),
                Is.EquivalentTo(sections));
        }
    }
}
