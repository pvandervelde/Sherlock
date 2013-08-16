//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines possible outcomes of test execution.
    /// </summary>
    public enum TestExecutionResult
    {
        /// <summary>
        /// The test execution had no outcome. Generally not a valid value.
        /// </summary>
        None,

        /// <summary>
        /// The test execution resulted in a passing test.
        /// </summary>
        Passed,

        /// <summary>
        /// The test execution resulted in a failing test.
        /// </summary>
        Failed,
    }
}
