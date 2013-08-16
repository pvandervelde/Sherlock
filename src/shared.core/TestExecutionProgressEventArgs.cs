//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores information about the progress of an executing test.
    /// </summary>
    [Serializable]
    public sealed class TestExecutionProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the ID of the test.
        /// </summary>
        private readonly int m_Id;

        /// <summary>
        /// The name of the report section under which the current section should be stored.
        /// </summary>
        private readonly string m_SectionName;

        /// <summary>
        /// The report section.
        /// </summary>
        private readonly TestSection m_Section;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionProgressEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID of the test. Is allowed to be <see langword="null" />.</param>
        /// <param name="sectionName">The name of the report section under which the current section should be stored.</param>
        /// <param name="section">The report test section.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sectionName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="section"/> is <see langword="null" />.
        /// </exception>
        public TestExecutionProgressEventArgs(int id, string sectionName, TestSection section)
        {
            {
                Lokad.Enforce.Argument(() => sectionName);
                Lokad.Enforce.Argument(() => sectionName, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => section);
            }

            m_Id = id;
            m_SectionName = sectionName;
            m_Section = section;
        }

        /// <summary>
        /// Gets the ID of the test.
        /// </summary>
        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the name of the report section under which the current section should be stored.
        /// </summary>
        public string SectionName
        {
            get
            {
                return m_SectionName;
            }
        }

        /// <summary>
        /// Gets the report test section.
        /// </summary>
        public TestSection Section
        {
            get
            {
                return m_Section;
            }
        }
    }
}
