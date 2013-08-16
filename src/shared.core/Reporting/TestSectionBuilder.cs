//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sherlock.Shared.Core.Properties;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Builds <see cref="TestSection"/> objects.
   /// </summary>
   public sealed class TestSectionBuilder : ITestSectionBuilder
   {
      /// <summary>
      /// The name of the <c>ReportSection</c> under which this <c>TestSection</c>
      /// should be stored.
      /// </summary>
      private readonly string m_ReportSectionName;

      /// <summary>
      /// The action that is invoked when a <c>TestSection</c> needs to
      /// be stored.
      /// </summary>
      private readonly Action<string, TestSection> m_OnStore;

      /// <summary>
      /// The collection of information messages.
      /// </summary>
      private readonly List<DateBasedTestInformation> m_InformationMessages =
         new List<DateBasedTestInformation>();

      /// <summary>
      /// The collection of warning messages.
      /// </summary>
      private readonly List<DateBasedTestInformation> m_WarningMessages =
         new List<DateBasedTestInformation>();

      /// <summary>
      /// The collection of error messages.
      /// </summary>
      private readonly List<DateBasedTestInformation> m_ErrorMessages =
         new List<DateBasedTestInformation>();

      /// <summary>
      /// The name of the <c>TestSection</c>.
      /// </summary>
      private string m_Name;

      /// <summary>
      /// The time at which the information gathering for the
      /// test section was started.
      /// </summary>
      private DateTimeOffset m_StartTime = DateTimeOffset.MinValue;

      /// <summary>
      /// Initializes a new instance of the <see cref="TestSectionBuilder"/> class.
      /// </summary>
      /// <param name="reportSectionToStoreUnder">
      ///   The name of the <see cref="ReportSection"/> under which the <see cref="TestSection"/>
      ///   should be stored.
      /// </param>
      /// <param name="onStore">
      ///   The action that is invoked when the <see cref="TestSection"/> should
      ///   be stored.
      /// </param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="reportSectionToStoreUnder"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="reportSectionToStoreUnder"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="onStore"/> is <see langword="null"/>.
      /// </exception>
      public TestSectionBuilder(string reportSectionToStoreUnder, Action<string, TestSection> onStore)
      {
         {
             Lokad.Enforce.Argument(() => reportSectionToStoreUnder);
             Lokad.Enforce.Argument(() => reportSectionToStoreUnder, Lokad.Rules.StringIs.NotEmpty);

             Lokad.Enforce.Argument(() => onStore);
         }

         m_ReportSectionName = reportSectionToStoreUnder;
         m_OnStore = onStore;
      }

      /// <summary>
      /// Gets a value indicating whether the builder was initialized.
      /// </summary>
      /// <value>
      ///   <see langword="true"/> if the builder was initialized; otherwise, <see langword="false"/>.
      /// </value>
      [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
      private bool WasInitialized
      {
         get
         {
            return !m_StartTime.Equals(DateTimeOffset.MinValue);
         }
      }

      /// <summary>
      /// Initializes the builder. This resets all the
      /// internal data structures and prepares them for
      /// the reception of data for a new <see cref="TestSection"/>.
      /// </summary>
      /// <param name="name">The name of the test section.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="name"/> is <see langword="null" />.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="name"/> is an empty string.
      /// </exception>
      public void Initialize(string name)
      {
         Initialize(name, DateTimeOffset.Now);
      }

      /// <summary>
      /// Initializes the builder. This resets all the
      /// internal data structures and prepares them for
      /// the reception of data for a new <see cref="TestSection"/>.
      /// </summary>
      /// <param name="name">The name of the test section.</param>
      /// <param name="startTime">The start time.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="name"/> is <see langword="null" />.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="name"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="startTime"/> is equal to <see cref="DateTimeOffset.MinValue"/>.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="startTime"/> is equal to <see cref="DateTimeOffset.MaxValue"/>.
      /// </exception>
      public void Initialize(string name, DateTimeOffset startTime)
      {
         {
             Lokad.Enforce.Argument(() => name);
             Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);

             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !startTime.Equals(DateTimeOffset.MinValue),
               Resources.Exceptions_Messages_ArgumentOutOfRange,
               startTime);
             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !startTime.Equals(DateTimeOffset.MaxValue),
               Resources.Exceptions_Messages_ArgumentOutOfRange,
               startTime);
         }

         Clear();
         m_Name = name;
         m_StartTime = startTime;
      }

      /// <summary>
      /// Clears the reporting data.
      /// </summary>
      private void Clear()
      {
         m_Name = string.Empty;
         m_InformationMessages.Clear();
         m_WarningMessages.Clear();
         m_ErrorMessages.Clear();

         m_StartTime = DateTimeOffset.MinValue;
      }

      /// <summary>
      /// Adds the information message to the collection of information
      /// messages.
      /// </summary>
      /// <param name="informationMessage">The information message.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="informationMessage"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="informationMessage"/> is an empty string.
      /// </exception>
      /// <exception cref="CannotAddMessageToUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method has not been called.
      /// </exception>
      public void AddInformationMessage(string informationMessage)
      {
         AddInformationMessage(DateTimeOffset.Now, informationMessage);
      }

      /// <summary>
      /// Adds the information message.
      /// </summary>
      /// <param name="time">The time on which the message was logged.</param>
      /// <param name="informationMessage">The information message.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="informationMessage"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="informationMessage"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="time"/> is equal to <see cref="DateTimeOffset.MinValue"/>.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="time"/> is equal to <see cref="DateTimeOffset.MaxValue"/>.
      /// </exception>
      /// <exception cref="CannotAddMessageToUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method has not been called.
      /// </exception>
      public void AddInformationMessage(DateTimeOffset time, string informationMessage)
      {
         {
             Lokad.Enforce.Argument(() => informationMessage);
             Lokad.Enforce.Argument(() => informationMessage, Lokad.Rules.StringIs.NotEmpty);

             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !time.Equals(DateTimeOffset.MinValue),
               Resources.Exceptions_Messages_ArgumentOutOfRange,
               time);
             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !time.Equals(DateTimeOffset.MaxValue),
               Resources.Exceptions_Messages_ArgumentOutOfRange,
               time);
         }

         if (!WasInitialized)
         {
            throw new CannotAddMessageToUninitializedTestSectionException();
         }

         m_InformationMessages.Add(new DateBasedTestInformation(time, informationMessage));
      }

      /// <summary>
      /// Adds the warning message to the collection of warning
      /// messages.
      /// </summary>
      /// <param name="warningMessage">The warning message.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="warningMessage"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="warningMessage"/> is an empty string.
      /// </exception>
      /// <exception cref="CannotAddMessageToUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method has not been called.
      /// </exception>
      public void AddWarningMessage(string warningMessage)
      {
         AddWarningMessage(DateTimeOffset.Now, warningMessage);
      }

      /// <summary>
      /// Adds the warning message.
      /// </summary>
      /// <param name="time">The time on which the message was logged.</param>
      /// <param name="warningMessage">The warning message.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="warningMessage"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="warningMessage"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="time"/> is equal to <see cref="DateTimeOffset.MinValue"/>.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="time"/> is equal to <see cref="DateTimeOffset.MaxValue"/>.
      /// </exception>
      /// <exception cref="CannotAddMessageToUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method has not been called.
      /// </exception>
      public void AddWarningMessage(DateTimeOffset time, string warningMessage)
      {
         {
             Lokad.Enforce.Argument(() => warningMessage);
             Lokad.Enforce.Argument(() => warningMessage, Lokad.Rules.StringIs.NotEmpty);

             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !time.Equals(DateTimeOffset.MinValue), 
               Resources.Exceptions_Messages_ArgumentOutOfRange, 
               time);
             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !time.Equals(DateTimeOffset.MaxValue), 
               Resources.Exceptions_Messages_ArgumentOutOfRange, 
               time);
         }

         if (!WasInitialized)
         {
            throw new CannotAddMessageToUninitializedTestSectionException();
         }

         m_WarningMessages.Add(new DateBasedTestInformation(time, warningMessage));
      }

      /// <summary>
      /// Adds the error message to the collection of error messages.
      /// </summary>
      /// <param name="errorMessage">The error message.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="errorMessage"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="errorMessage"/> is an empty string.
      /// </exception>
      /// <exception cref="CannotAddMessageToUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method has not been called.
      /// </exception>
      public void AddErrorMessage(string errorMessage)
      {
         AddErrorMessage(DateTimeOffset.Now, errorMessage);
      }

      /// <summary>
      /// Adds the error message.
      /// </summary>
      /// <param name="time">The time on which the message was logged.</param>
      /// <param name="errorMessage">The error message.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="errorMessage"/> is <see langword="null"/>.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="errorMessage"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="time"/> is equal to <see cref="DateTimeOffset.MinValue"/>.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      ///   Thrown when <paramref name="time"/> is equal to <see cref="DateTimeOffset.MaxValue"/>.
      /// </exception>
      /// <exception cref="CannotAddMessageToUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method has not been called.
      /// </exception>
      public void AddErrorMessage(DateTimeOffset time, string errorMessage)
      {
         {
             Lokad.Enforce.Argument(() => errorMessage);
             Lokad.Enforce.Argument(() => errorMessage, Lokad.Rules.StringIs.NotEmpty);

             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !time.Equals(DateTimeOffset.MinValue), 
               Resources.Exceptions_Messages_ArgumentOutOfRange, 
               time);
             Lokad.Enforce.With<ArgumentOutOfRangeException>(
               !time.Equals(DateTimeOffset.MaxValue), 
               Resources.Exceptions_Messages_ArgumentOutOfRange, 
               time);
         }

         if (!WasInitialized)
         {
            throw new CannotAddMessageToUninitializedTestSectionException();
         }

         m_ErrorMessages.Add(new DateBasedTestInformation(time, errorMessage));
      }

      /// <summary>
      /// Finalizes the test section data and passes it on to a storage object.
      /// </summary>
      /// <param name="wasSuccessful">
      /// Set to <see langword="true"/> if all tests in the section were successful.
      /// </param>
      /// <exception cref="CannotFinalizeAnUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method was not called prior to
      ///   calling <see cref="FinalizeAndStore(bool)"/>.
      /// </exception>
      public void FinalizeAndStore(bool wasSuccessful)
      {
         FinalizeAndStore(wasSuccessful, DateTimeOffset.Now);
      }

      /// <summary>
      /// Finalizes the test section data and passes it on to a storage object.
      /// </summary>
      /// <param name="wasSuccessful">Set to <see langword="true"/> if all tests in the section were successful.</param>
      /// <param name="endTime">The end time.</param>
      /// <exception cref="CannotFinalizeAnUninitializedTestSectionException">
      ///   Thrown when the <c>Initialize</c> method was not called prior to
      ///   calling <see cref="FinalizeAndStore(bool, DateTimeOffset)"/>.
      /// </exception>
      public void FinalizeAndStore(bool wasSuccessful, DateTimeOffset endTime)
      {
         if (!WasInitialized)
         {
            throw new CannotFinalizeAnUninitializedTestSectionException();
         }

         var section = new TestSection(
            m_Name,
            m_StartTime,
            endTime,
            wasSuccessful,
            m_InformationMessages,
            m_WarningMessages,
            m_ErrorMessages);

         m_OnStore(m_ReportSectionName, section);

         // Clear the data. There's no way to finalize twice!
         Clear();
      }
   }
}
