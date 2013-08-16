//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Stores data describing a single test for a specific section.
   /// </summary>
   [Serializable]
   public sealed class TestSection
   {
      /// <summary>
      /// The collection that holds the information messages.
      /// </summary>
      private readonly List<DateBasedTestInformation> m_Info = new List<DateBasedTestInformation>();

      /// <summary>
      /// The collection that holds the warning messages.
      /// </summary>
      private readonly List<DateBasedTestInformation> m_Warning = new List<DateBasedTestInformation>();

      /// <summary>
      /// The collection that holds the error messages.
      /// </summary>
      private readonly List<DateBasedTestInformation> m_Error = new List<DateBasedTestInformation>();

      /// <summary>
      /// Initializes a new instance of the <see cref="TestSection"/> class.
      /// </summary>
      /// <param name="name">The name of the test section.</param>
      /// <param name="start">The start time for the section.</param>
      /// <param name="end">The end time for the section.</param>
      /// <param name="success">If set to <see langword="true"/> [success].</param>
      /// <param name="infoMessages">The info messages.</param>
      /// <param name="warningMessages">The warning messages.</param>
      /// <param name="errorMessages">The error messages.</param>
      /// <exception cref="ArgumentNullException">
      /// Thrown when <paramref name="name"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown when <paramref name="name"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// Thrown when <paramref name="infoMessages"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// Thrown when <paramref name="warningMessages"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// Thrown when <paramref name="errorMessages"/> is <see langword="null"/>.
      /// </exception>
      public TestSection(
         string name,
         DateTimeOffset start,
         DateTimeOffset end,
         bool success,
         IEnumerable<DateBasedTestInformation> infoMessages,
         IEnumerable<DateBasedTestInformation> warningMessages,
         IEnumerable<DateBasedTestInformation> errorMessages)
      {
         {
             Lokad.Enforce.Argument(() => name);
             Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);

             Lokad.Enforce.Argument(() => infoMessages);
             Lokad.Enforce.Argument(() => warningMessages);
             Lokad.Enforce.Argument(() => errorMessages);
         }

         Name = name;

         StartTime = start;
         EndTime = end;
         WasSuccessful = success;

         m_Info.AddRange(infoMessages);
         m_Warning.AddRange(warningMessages);
         m_Error.AddRange(errorMessages);
      }

      /// <summary>
      /// Gets the name of the test section.
      /// </summary>
      public string Name
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the date and time the test started.
      /// </summary>
      public DateTimeOffset StartTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets date and time the test ended.
      /// </summary>
      public DateTimeOffset EndTime
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets a value indicating whether the test was successful.
      /// </summary>
      /// <value>
      ///   <see langword="true"/> if the test was successful; otherwise, <see langword="false"/>.
      /// </value>
      [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
      public bool WasSuccessful
      {
         get;
         private set;
      }

      /// <summary>
      /// Returns the collection of information messages.
      /// </summary>
      /// <returns>
      /// The collection that holds the information messages.
      /// </returns>
      public IEnumerable<DateBasedTestInformation> InfoMessages()
      {
         return m_Info.AsReadOnly();
      }

      /// <summary>
      /// Returns the collection of warning messages.
      /// </summary>
      /// <returns>
      /// The collection that holds the warning messages.
      /// </returns>
      public IEnumerable<DateBasedTestInformation> WarningMessages()
      {
         return m_Warning.AsReadOnly();
      }

      /// <summary>
      /// Returns the collection of error messages.
      /// </summary>
      /// <returns>
      /// The collection that holds the error messages.
      /// </returns>
      public IEnumerable<DateBasedTestInformation> ErrorMessages()
      {
         return m_Error.AsReadOnly();
      }
   }
}
