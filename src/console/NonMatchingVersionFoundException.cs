//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Sherlock.Console.Properties;

namespace Sherlock.Console
{
    /// <summary>
    /// An exception thrown when the application is not able to find a matching <see cref="IVersionedConfigurationReader"/>
    /// for the version in the configuration file.
    /// </summary>
    [Serializable]
    public sealed class NonMatchingVersionFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingVersionFoundException"/> class.
        /// </summary>
        public NonMatchingVersionFoundException()
            : this(Resources.Exceptions_Messages_NonMatchingVersionFound)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingVersionFoundException"/> class.
        /// </summary>
        /// <param name="configurationVersion">The configuration version.</param>
        /// <param name="applicationVersion">The application version.</param>
        public NonMatchingVersionFoundException(string configurationVersion, string applicationVersion)
            : this(
               string.Format(
                  CultureInfo.InvariantCulture,
                  Resources.Exceptions_Messages_NonMatchingVersionFound_WithVersions,
                  configurationVersion,
                  applicationVersion))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingVersionFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NonMatchingVersionFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingVersionFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NonMatchingVersionFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingVersionFoundException"/> class.
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
        private NonMatchingVersionFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
