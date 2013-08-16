//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the interface for objects that control the test cycle from processing the test request to the completion of the test.
    /// </summary>
    internal interface ICycleTestsFromRequestToCompletion
    {
        /// <summary>
        /// Starts the test cycle.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the test cycle.
        /// </summary>
        void Stop();
    }
}
