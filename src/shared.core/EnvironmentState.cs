//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the state of an active environment.
    /// </summary>
    public sealed class EnvironmentState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentState"/> class.
        /// </summary>
        /// <param name="testState">The current state of the test.</param>
        public EnvironmentState(TestExecutionState testState)
        {
            TestState = testState;
        }

        /// <summary>
        /// Gets the execution state of the test that is currently being executed.
        /// </summary>
        public TestExecutionState TestState
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}",
                TestState);
        }
    }
}
