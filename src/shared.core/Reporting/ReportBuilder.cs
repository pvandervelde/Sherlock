//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sherlock.Shared.Core.Reporting
{
    /// <summary>
    /// Defines methods used for the generation of test reports.
    /// </summary>
    public sealed class ReportBuilder : IReportBuilder
    {
        /// <summary>
        /// The collection that maps the <see cref="TestSection"/> objects to
        /// their respective report section.
        /// </summary>
        private readonly SortedList<string, List<TestSection>> m_Sections =
            new SortedList<string, List<TestSection>>();

        /// <summary>
        /// The time that the current test run was started.
        /// </summary>
        private DateTimeOffset m_StartTime = DateTimeOffset.MinValue;

        /// <summary>
        /// The time the current test run was finished.
        /// </summary>
        private DateTimeOffset m_EndTime = DateTimeOffset.MinValue;

        /// <summary>
        /// The name of the product that is being tested.
        /// </summary>
        private string m_ProductName;

        /// <summary>
        /// The version of the product that is being tested.
        /// </summary>
        private string m_ProductVersion;

        /// <summary>
        /// The name of the user who started the test.
        /// </summary>
        private string m_TestOwner;

        /// <summary>
        /// The description of the test.
        /// </summary>
        private string m_TestDescription;

        /// <summary>
        /// Initializes a new report.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <param name="productVersion">The version of the product.</param>
        /// <param name="owner">The name of the user that started the test.</param>
        /// <param name="description">The description for the test.</param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="productName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="productName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="productVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="owner"/> is an empty string.
        /// </exception>
        public void InitializeNewReport(
            string productName,
            string productVersion,
            string owner,
            string description)
        {
            {
                Lokad.Enforce.Argument(() => productName);
                Lokad.Enforce.Argument(() => productName, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => productVersion);

                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => owner, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Sections.Clear();

            m_StartTime = DateTimeOffset.Now;
            m_EndTime = DateTimeOffset.MinValue;

            m_ProductName = productName;
            m_ProductVersion = productVersion;
            m_TestOwner = owner;
            m_TestDescription = description;
        }

        /// <summary>
        /// Adds new test information to the <see cref="ReportSection"/> of the given name.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="testSections">The collection that holds the test sections.</param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="name"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="testSections"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotAddSectionToFinalizedReportException">
        ///   Thrown when the report has already been finalized.
        /// </exception>
        public void AddToSection(string name, IEnumerable<TestSection> testSections)
        {
            {
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => testSections);
            }

            if (m_StartTime.Equals(DateTimeOffset.MinValue))
            {
                throw new CannotAddSectionToUninitializedReportException();
            }

            if (HasReportBeenFinalized)
            {
                throw new CannotAddSectionToFinalizedReportException();
            }

            if (!testSections.Exists())
            {
                return;
            }

            if (!m_Sections.ContainsKey(name))
            {
                m_Sections.Add(name, new List<TestSection>());
            }

            var list = m_Sections[name];
            list.AddRange(testSections);
        }

        /// <summary>
        /// Finalizes the current report.
        /// </summary>
        public void FinalizeReport()
        {
            if (m_StartTime.Equals(DateTimeOffset.MinValue))
            {
                throw new CannotFinalizeAnUninitializedReportException();
            }

            if (HasReportBeenFinalized)
            {
                throw new CannotRefinalizeReportException();
            }

            m_EndTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has report been finalized.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance has report been finalized; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasReportBeenFinalized
        {
            get
            {
                return !m_EndTime.Equals(DateTimeOffset.MinValue);
            }
        }

        /// <summary>
        /// Builds the report.
        /// </summary>
        /// <returns>A new instance of the current report.</returns>
        public IReport Build()
        {
            if (m_StartTime.Equals(DateTimeOffset.MinValue))
            {
                throw new CannotBuildAnUninitializedReportException();
            }

            if (!HasReportBeenFinalized)
            {
                throw new CannotBuildNonFinalizedReportException();
            }

            var header = new ReportHeader(
                m_StartTime,
                m_EndTime,
                m_ProductName,
                m_ProductVersion,
                m_TestOwner,
                m_TestDescription);

            var sections = from map in m_Sections
                select new ReportSection(map.Key, map.Value);

            return new Report(header, sections);
        }
    }
}
