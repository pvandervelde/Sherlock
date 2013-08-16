//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Sherlock.Shared.DataAccess.Properties;

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// An exception thrown when there is a configuration value missing.
    /// </summary>
    [Serializable]
    public sealed class InvalidSchemaVersionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSchemaVersionException"/> class.
        /// </summary>
        public InvalidSchemaVersionException()
            : this(Resources.Exceptions_Messages_InvalidSchemaVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSchemaVersionException"/> class.
        /// </summary>
        /// <param name="expected">The expected version.</param>
        /// <param name="actual">The actual version.</param>
        public InvalidSchemaVersionException(int expected, int actual)
            : this(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Exceptions_Messages_InvalidSchemaVersion_WithVersions,
                    expected,
                    actual))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSchemaVersionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidSchemaVersionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSchemaVersionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidSchemaVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSchemaVersionException"/> class.
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
        private InvalidSchemaVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
