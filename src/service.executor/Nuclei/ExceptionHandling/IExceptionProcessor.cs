//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Service.Executor.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the interface for objects that process unhandled exceptions.
    /// </summary>
    public interface IExceptionProcessor : IDisposable
    {
        /// <summary>
        /// Processes the given exception.
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        void Process(Exception exception);
    }
}

