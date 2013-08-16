//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Sherlock.Shared.Core.Properties;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// An exception which is thrown when a machine environment is already in use.
    /// </summary>
    [Serializable]
    public sealed class EnvironmentAlreadyInUseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentAlreadyInUseException"/> class.
        /// </summary>
        public EnvironmentAlreadyInUseException()
            : this(Resources.Exceptions_Messages_EnvironmentAlreadyInUse)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentAlreadyInUseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EnvironmentAlreadyInUseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentAlreadyInUseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EnvironmentAlreadyInUseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentAlreadyInUseException"/> class.
        /// </summary>
        /// <param name="info">
        ///   The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///   data about the exception being thrown.
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
        private EnvironmentAlreadyInUseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
