//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores a description of an x-copy test step.
    /// </summary>
    internal sealed class XCopyTestStepDescription : TestStepDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XCopyTestStepDescription"/> class.
        /// </summary>
        /// <param name="environment">The name of the environment to which the current step belongs.</param>
        /// <param name="order">The index of the test step in the test sequence.</param>
        /// <param name="destination">The full path to the directory where the files and directories should be copied to.</param>
        public XCopyTestStepDescription(string environment, int order, string destination)
            : base(environment, order, Enumerable.Empty<TestStepParameterDescription>())
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(destination), "The destination directory should not be an empty string.");
            }

            Destination = destination;
        }

        /// <summary>
        /// Gets the full path to the directory where the files and directories should be copied to.
        /// </summary>
        public string Destination
        {
            get;
            private set;
        }
    }
}
