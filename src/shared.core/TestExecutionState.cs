//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines all the possible states a test can be in.
    /// </summary>
    public enum TestExecutionState
    {
        /// <summary>
        /// The test has no state. Generally not a valid value.
        /// </summary>
        None,

        /// <summary>
        /// The test is ready for execution but has not been started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The test is being executed.
        /// </summary>
        Running,

        /// <summary>
        /// The test crashed during execution.
        /// </summary>
        Crashed,

        /// <summary>
        /// The test completed but failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The test completed and passed.
        /// </summary>
        Passed,

        /// <summary>
        /// The test is in an unknown state.
        /// </summary>
        Unknown,
    }
}
