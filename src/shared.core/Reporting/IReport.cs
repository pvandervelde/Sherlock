//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines the interface for a test report.
   /// </summary>
   /// <design>
   /// <para>
   /// The test process consists of several different 'sections'.
   /// Each section describes one part of the test process. The
   /// available sections are
   /// </para>
   /// <list type="table">
   /// <listheader>
   ///   <term>Section</term>
   ///   <description>Description</description>
   /// </listheader>
   /// <item>
   ///   <term>Machine</term>
   ///   <description>The part of the test process where the desired machine is started.</description>
   /// </item>
   /// <item>
   ///   <term>Setup</term>
   ///   <description>
   ///      The part of the test process where the required binaries are transfered to the machine
   ///      and installed if necessary.
   ///   </description>
   /// </item>
   /// <item>
   ///   <term>Test</term>
   ///   <description>The part of the test process where the actual tests are run.</description>
   /// </item>
   /// </list>
   /// <para>
   /// The test report consists of a header, which describes
   /// the general conditions of the test, and a set of test
   /// sections, which describe the results for the different
   /// sections of the test process.
   /// </para>
   /// </design>
   public interface IReport
   {
      /// <summary>
      /// Gets the header for the report.
      /// </summary>
      ReportHeader Header
      {
         get;
      }

      /// <summary>
      /// Returns the collection of test sections that describe the test results
      /// for each of the different sections.
      /// </summary>
      /// <returns>
      /// The collection of test sections with their results.
      /// </returns>
      IEnumerable<ReportSection> Sections();
   }
}
