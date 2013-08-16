//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines the interface for objects that can build a report of a single
   /// test session.
   /// </summary>
   public interface IReportBuilder
   {
      /// <summary>
      /// Initializes a new report.
      /// </summary>
      /// <param name="productName">Name of the product.</param>
      /// <param name="productVersion">The version of the product.</param>
      void InitializeNewReport(string productName, string productVersion);

      /// <summary>
      /// Adds new test information to the <see cref="ReportSection"/> of the given name.
      /// </summary>
      /// <param name="name">The name of the section.</param>
      /// <param name="testSections">The collection that holds the test sections.</param>
      void AddToSection(string name, IEnumerable<TestSection> testSections);

      /// <summary>
      /// Finalizes the current report.
      /// </summary>
      void FinalizeReport();

      /// <summary>
      /// Gets a value indicating whether this instance has report been finalized.
      /// </summary>
      /// <value>
      ///   <see langword="true"/> if this instance has report been finalized; otherwise, <see langword="false"/>.
      /// </value>
      [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
      bool HasReportBeenFinalized
      {
         get;
      }

      /// <summary>
      /// Builds the report.
      /// </summary>
      /// <returns>
      /// A new instance of the current report.
      /// </returns>
      IReport Build();
   }
}
