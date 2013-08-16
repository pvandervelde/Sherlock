//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Sherlock.Console.Properties;

namespace Sherlock.Console
{
    /// <summary>
    /// An exception thrown when the application is not able to find a notification element in the configuration file.
    /// </summary>
    [Serializable]
    public sealed class MissingNotificationConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingNotificationConfigurationException"/> class.
        /// </summary>
        public MissingNotificationConfigurationException()
            : this(Resources.Exceptions_Messages_MissingNotificationConfiguration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingNotificationConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingNotificationConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingNotificationConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingNotificationConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingNotificationConfigurationException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        ///     object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private MissingNotificationConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
