//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that are able to clean-up an environment after one or more
    /// test steps have been executed.
    /// </summary>
    public interface IParticipateInCleanUp
    {
        /// <summary>
        /// Cleans the environment after the test step that produced the given state.
        /// </summary>
        /// <param name="stepToCleanUp">The test step from which the state should be cleaned.</param>
        void CleanUp(TestStep stepToCleanUp);
    }
}
