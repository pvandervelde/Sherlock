//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the possible exit results for a guarded execution of a method.
    /// </summary>
    internal enum GuardResult
    {
        /// <summary>
        /// There was no exit result. Not normally a valid value.
        /// </summary>
        None,

        /// <summary>
        /// The method executed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The method execution failed at some point with an unhandled exception.
        /// </summary>
        Failure,
    }
}

