//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Sherlock.Service.Properties;

namespace Sherlock.Service
{
    /// <summary>
    /// An exception thrown when the <see cref="ApplicationTrackingJob"/> fails to link
    /// a process to the existing job.
    /// </summary>
    [Serializable]
    public sealed class UnableToLinkChildProcessToJobException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLinkChildProcessToJobException"/> class.
        /// </summary>
        public UnableToLinkChildProcessToJobException()
            : this(Resources.Exceptions_Messages_UnableToLinkChildProcessToJob)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLinkChildProcessToJobException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnableToLinkChildProcessToJobException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLinkChildProcessToJobException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnableToLinkChildProcessToJobException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToLinkChildProcessToJobException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        ///     about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private UnableToLinkChildProcessToJobException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
