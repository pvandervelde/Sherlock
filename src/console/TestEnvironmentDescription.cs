//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the description of a test environment.
    /// </summary>
    internal sealed class TestEnvironmentDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironmentDescription"/> class.
        /// </summary>
        /// <param name="name">The name of the test environment.</param>
        /// <param name="operatingSystem">The operating system required for the test environment.</param>
        /// <param name="applications">The collection of applications that are required for the test environment.</param>
        public TestEnvironmentDescription(string name, OperatingSystemDescription operatingSystem, IEnumerable<ApplicationDescription> applications)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(name), "The name of the environment should not be an empty string.");
                Debug.Assert(operatingSystem != null, "The operating system should not be a null reference.");
                Debug.Assert(applications != null, "The collection of applications should not be a null reference.");
            }

            Name = name;
            OperatingSystem = operatingSystem;
            Applications = applications;
        }

        /// <summary>
        /// Gets or sets the ID of the environment.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the operating system that is required for the environment.
        /// </summary>
        public OperatingSystemDescription OperatingSystem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection containing all the desired applications for the environment.
        /// </summary>
        public IEnumerable<ApplicationDescription> Applications
        {
            get;
            private set;
        }
    }
}
