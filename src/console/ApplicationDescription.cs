//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores the description of an installed application.
    /// </summary>
    internal sealed class ApplicationDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDescription"/> class.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="version">The version of the application.</param>
        public ApplicationDescription(string name, Version version)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(name), "The name should not be an empty string.");
                Debug.Assert(version != null, "The version should not be null reference.");
            }

            Name = name;
            Version = version;
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public Version Version
        {
            get;
            private set;
        }
    }
}
