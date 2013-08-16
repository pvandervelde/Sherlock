//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Lokad;
using Lokad.Rules;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines a section of a report for a specific part of the test
   /// sequence.
   /// </summary>
   /// <remarks>
   /// <para>
   /// The available sections are
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
   ///      The part of the test process where the required binaries are transferred to the machine
   ///      and installed if necessary.
   ///   </description>
   /// </item>
   /// <item>
   ///   <term>Test</term>
   ///   <description>The part of the test process where the actual tests are run.</description>
   /// </item>
   /// </list>
   /// </remarks>
   [Serializable]
   public sealed class ReportSection
   {
      /// <summary>
      /// The collection that holds the <see cref="TestSection"/> objects
      /// for the current section.
      /// </summary>
      private readonly List<TestSection> m_Sections = new List<TestSection>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ReportSection"/> class.
      /// </summary>
      /// <param name="name">The name of the section.</param>
      /// <param name="tests">
      ///   The collection of <see cref="TestSection"/> objects that. belong
      ///   to the current section.
      /// </param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="name"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="name"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="tests"/> is <see langword="null"/>.
      /// </exception>
      public ReportSection(string name, IEnumerable<TestSection> tests)
      {
         {
            Enforce.Argument(() => name);
            Enforce.Argument(() => name, StringIs.NotEmpty);

            Enforce.Argument(() => tests);
         }

         Name = name;
         m_Sections.AddRange(tests);
      }

      /// <summary>
      /// Gets the name of the section.
      /// </summary>
      public string Name
      {
         get;
         private set;
      }

      /// <summary>
      /// Returns the collection of <see cref="TestSection"/> objects for the
      /// current section.
      /// </summary>
      /// <returns>
      /// The collection of <see cref="TestSection"/> objects.
      /// </returns>
      public IEnumerable<TestSection> Sections()
      {
         return m_Sections.AsReadOnly();
      }
   }
}
