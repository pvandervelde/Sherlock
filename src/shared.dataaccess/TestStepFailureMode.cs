//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Indicates what action should be taken when a test step fails in a test sequence.
    /// </summary>
    public enum TestStepFailureMode
    {
        /// <summary>
        /// No action should be taken. Generally not a valid option.
        /// </summary>
        None,

        /// <summary>
        /// The test sequence should be continued.
        /// </summary>
        Continue,

        /// <summary>
        /// The test sequence should be stopped.
        /// </summary>
        Stop,
    }
}
