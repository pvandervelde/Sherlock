//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// An <see cref="EventArgs"/> class that stores a <see cref="TestSection"/>
   /// that will be used in the test report.
   /// </summary>
   public sealed class ReportTestSectionEventArgs : EventArgs
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ReportTestSectionEventArgs"/> class.
      /// </summary>
      /// <param name="section">The section.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="section"/> is <see langword="null" />.
      /// </exception>
      public ReportTestSectionEventArgs(TestSection section)
      {
         {
            Lokad.Enforce.Argument(() => section);
         }

         Section = section;
      }

      /// <summary>
      /// Gets the test section for the report.
      /// </summary>
      /// <value>The test section.</value>
      public TestSection Section
      {
         get;
         private set;
      }
   }
}
