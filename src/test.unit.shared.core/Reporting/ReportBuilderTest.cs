//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ReportBuilderTest
    {
        [Test]
        public void InitializeWithEmptyProductName()
        {
            var builder = new ReportBuilder();
            Assert.Throws<ArgumentException>(
                () => builder.InitializeNewReport(
                    string.Empty,
                    "a",
                    "b",
                    "c"));
        }

        [Test]
        public void InitializeWithEmptyUserName()
        {
            var builder = new ReportBuilder();
            Assert.Throws<ArgumentException>(
                () => builder.InitializeNewReport(
                    "a",
                    "b",
                    string.Empty,
                    "c"));
        }

        [Test]
        public void AddToSectionWithEmptyName()
        {
            var builder = new ReportBuilder();
            Assert.Throws<ArgumentException>(() => builder.AddToSection(string.Empty, new List<TestSection>()));
        }

        [Test]
        public void AddToSectionWithUninitializedReport()
        {
            var builder = new ReportBuilder();
            Assert.Throws<CannotAddSectionToUninitializedReportException>(() => builder.AddToSection("bla", new List<TestSection>()));
        }

        [Test]
        public void AddToSectionWithFinalizedReport()
        {
            var builder = new ReportBuilder();
            builder.InitializeNewReport("product", "1.1", "user", "description");
            builder.FinalizeReport();

            Assert.Throws<CannotAddSectionToFinalizedReportException>(() => builder.AddToSection("bla", new List<TestSection>()));
        }

        [Test]
        public void FinalizeWithUninitializedReport()
        {
            var builder = new ReportBuilder();
            Assert.Throws<CannotFinalizeAnUninitializedReportException>(builder.FinalizeReport);
        }

        [Test]
        public void FinalizeWithFinalizedReport()
        {
            var builder = new ReportBuilder();
            builder.InitializeNewReport("product", "1.1", "user", "description");
            builder.FinalizeReport();

            Assert.Throws<CannotRefinalizeReportException>(builder.FinalizeReport);
        }

        [Test]
        public void BuildWithNonInitializedReport()
        {
            var builder = new ReportBuilder();
            Assert.Throws<CannotBuildAnUninitializedReportException>(() => builder.Build());
        }

        [Test]
        public void BuildWithNonFinalizedReport()
        {
            var builder = new ReportBuilder();
            builder.InitializeNewReport("product", "1.1", "user", "description");

            Assert.Throws<CannotBuildNonFinalizedReportException>(() => builder.Build());
        }

        [Test]
        public void Build()
        {
            var builder = new ReportBuilder();

            var product = "product";
            var version = "1.2.3.4";
            var user = "user";
            var description = "description";
            builder.InitializeNewReport(product, version, user, description);

            var name = "name";
            var sections = new List<TestSection>
                {
                    new TestSection(
                        "someName",
                        DateTimeOffset.Now,
                        DateTimeOffset.Now,
                        true,
                        new List<DateBasedTestInformation>(),
                        new List<DateBasedTestInformation>(),
                        new List<DateBasedTestInformation>())
                };
            builder.AddToSection(name, sections);

            builder.FinalizeReport();
            var report = builder.Build();

            Assert.AreEqual(product, report.Header.ProductName);
            Assert.AreEqual(version, report.Header.ProductVersion);
            Assert.AreEqual(user, report.Header.UserName);
            Assert.AreEqual(description, report.Header.Description);

            Assert.AreEqual(1, report.Sections().Count());
            Assert.That(
                report.Sections().First().Sections(),
                Is.EquivalentTo(sections));
        }
    }
}
