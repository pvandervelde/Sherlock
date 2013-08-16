//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines a test report.
   /// </summary>
   [Serializable]
   internal sealed class Report : IReport
   {
      /// <summary>
      /// The collection that holds the test information for each of the sections.
      /// </summary>
      private readonly List<ReportSection> m_Sections = new List<ReportSection>();

      /// <summary>
      /// Initializes a new instance of the <see cref="Report"/> class.
      /// </summary>
      /// <param name="header">The header for the report.</param>
      /// <param name="sections">The sections that contain the test results.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="header"/> is <see langword="null" />.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="sections"/> is <see langword="null" />.
      /// </exception>
      public Report(ReportHeader header, IEnumerable<ReportSection> sections)
      {
         {
             Lokad.Enforce.Argument(() => header);
             Lokad.Enforce.Argument(() => sections);
         }

         Header = header;
         m_Sections.AddRange(sections);
      }

      /// <summary>
      /// Gets the header for the report.
      /// </summary>
      public ReportHeader Header
      {
         get;
         private set;
      }

      /// <summary>
      /// Returns the collection of test sections that describe the test results
      /// for each of the different sections.
      /// </summary>
      /// <returns>
      /// The collection of test sections with their results.
      /// </returns>
      public IEnumerable<ReportSection> Sections()
      {
         return m_Sections.AsReadOnly();
      }
   }
}
