//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Service.Master.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the interface for objects which take care of the last chance exception handling 
    /// actions.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Used when an unhandled exception occurs in an <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="isApplicationTerminating">Indicates if the application is about to shut down or not.</param>
        void OnException(Exception exception, bool isApplicationTerminating);
    }
}
