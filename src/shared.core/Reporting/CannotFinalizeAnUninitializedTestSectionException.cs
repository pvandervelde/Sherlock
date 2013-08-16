//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Sherlock.Shared.Core.Properties;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// An exception thrown when a user tries to finalize a test section that has not
   /// been initialized.
   /// </summary>
   [Serializable]
   public sealed class CannotFinalizeAnUninitializedTestSectionException : Exception
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="CannotFinalizeAnUninitializedTestSectionException"/> class.
      /// </summary>
      public CannotFinalizeAnUninitializedTestSectionException()
         : this(Resources.Exceptions_Messages_CannotFinalizeAnUninitializedTestSection)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CannotFinalizeAnUninitializedTestSectionException"/> class.
      /// </summary>
      /// <param name="message">The message.</param>
      public CannotFinalizeAnUninitializedTestSectionException(string message)
         : base(message)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CannotFinalizeAnUninitializedTestSectionException"/> class.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="innerException">The inner exception.</param>
      public CannotFinalizeAnUninitializedTestSectionException(string message, Exception innerException)
         : base(message, innerException)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CannotFinalizeAnUninitializedTestSectionException"/> class.
      /// </summary>
      /// <param name="info">
      ///   The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data
      ///   about the exception being thrown.
      /// </param>
      /// <param name="context">
      ///   The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
      ///   about the source or destination.
      /// </param>
      /// <exception cref="T:System.ArgumentNullException">
      /// The <paramref name="info"/> parameter is null.
      /// </exception>
      /// <exception cref="T:System.Runtime.Serialization.SerializationException">
      /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
      /// </exception>
      private CannotFinalizeAnUninitializedTestSectionException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }
   }
}
