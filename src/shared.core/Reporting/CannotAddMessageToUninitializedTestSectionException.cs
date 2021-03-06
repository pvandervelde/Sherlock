﻿//-----------------------------------------------------------------------
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
   /// An exception thrown when a user tries add a message to a test section that has not
   /// been initialized.
   /// </summary>
   [Serializable]
   public sealed class CannotAddMessageToUninitializedTestSectionException : Exception
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="CannotAddMessageToUninitializedTestSectionException"/> class.
      /// </summary>
      public CannotAddMessageToUninitializedTestSectionException()
         : this(Resources.Exceptions_Messages_CannotAddMessageToUninitializedTestSection)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CannotAddMessageToUninitializedTestSectionException"/> class.
      /// </summary>
      /// <param name="message">The message.</param>
      public CannotAddMessageToUninitializedTestSectionException(string message)
         : base(message)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CannotAddMessageToUninitializedTestSectionException"/> class.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="innerException">The inner exception.</param>
      public CannotAddMessageToUninitializedTestSectionException(string message, Exception innerException)
         : base(message, innerException)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CannotAddMessageToUninitializedTestSectionException"/> class.
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
      private CannotAddMessageToUninitializedTestSectionException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }
   }
}
