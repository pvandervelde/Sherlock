//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines the interface for objects that construct
   /// <see cref="TestSection"/> objects.
   /// </summary>
   public interface ITestSectionBuilder
   {
      /// <summary>
      /// Initializes the builder. This resets all the
      /// internal data structures and prepares them for
      /// the reception of data for a new <see cref="TestSection"/>.
      /// </summary>
      /// <param name="name">The name of the test section.</param>
      void Initialize(string name);

      /// <summary>
      /// Initializes the builder. This resets all the
      /// internal data structures and prepares them for
      /// the reception of data for a new <see cref="TestSection"/>.
      /// </summary>
      /// <param name="name">The name of the test section.</param>
      /// <param name="startTime">The start time.</param>
      void Initialize(string name, DateTimeOffset startTime);

      /// <summary>
      /// Adds the information message to the collection of information
      /// messages.
      /// </summary>
      /// <param name="informationMessage">The information message.</param>
      void AddInformationMessage(string informationMessage);

      /// <summary>
      /// Adds the information message to the collection of information
      /// messages.
      /// </summary>
      /// <param name="time">The time for the message.</param>
      /// <param name="informationMessage">The information message.</param>
      void AddInformationMessage(DateTimeOffset time, string informationMessage);

      /// <summary>
      /// Adds the warning message to the collection of warning
      /// messages.
      /// </summary>
      /// <param name="warningMessage">The warning message.</param>
      void AddWarningMessage(string warningMessage);

      /// <summary>
      /// Adds the warning message to the collection of warning
      /// messages.
      /// </summary>
      /// <param name="time">The time for the message.</param>
      /// <param name="warningMessage">The warning message.</param>
      void AddWarningMessage(DateTimeOffset time, string warningMessage);

      /// <summary>
      /// Adds the error message to the collection of error messages.
      /// </summary>
      /// <param name="errorMessage">The error message.</param>
      void AddErrorMessage(string errorMessage);

      /// <summary>
      /// Adds the error message to the collection of error messages.
      /// </summary>
      /// <param name="time">The time for the message.</param>
      /// <param name="errorMessage">The error message.</param>
      void AddErrorMessage(DateTimeOffset time, string errorMessage);

      /// <summary>
      /// Finalizes the test section data and passes it on to a storage object.
      /// </summary>
      /// <param name="wasSuccessful">
      /// Set to <see langword="true"/> if all tests in the section were successful.
      /// </param>
      void FinalizeAndStore(bool wasSuccessful);

      /// <summary>
      /// Finalizes the test section data and passes it on to a storage object.
      /// </summary>
      /// <param name="wasSuccessful">Set to <see langword="true"/> if all tests in the section were successful.</param>
      /// <param name="endTime">The end time.</param>
      void FinalizeAndStore(bool wasSuccessful, DateTimeOffset endTime);
   }
}
