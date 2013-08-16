//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sherlock.Executor.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines a top level exception handler which stops all exceptions from propagating out of the application, thus
    /// providing a chance for logging and semi-graceful termination of the application.
    /// </summary>
    internal static class TopLevelExceptionGuard
    {
        /// <summary>
        /// Runs an action inside a high level try .. catch construct that will not let any errors escape
        /// but will log errors to a file and the event log.
        /// </summary>
        /// <param name="actionToExecute">The action that should be executed.</param>
        /// <param name="exceptionProcessors">The collection of exception processors that will be used to log any unhandled exception.</param>
        /// <returns>
        /// A value indicating whether the action executed successfully or not.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching an Exception object here because this is the top-level exception handler.")]
        public static GuardResult RunGuarded(Action actionToExecute, params IExceptionProcessor[] exceptionProcessors)
        {
            {
                Debug.Assert(actionToExecute != null, "The application method should not be null.");
            }

            using (var processor = new ExceptionHandler(exceptionProcessors))
            {
                GuardResult result = GuardResult.None;
                ExceptionFilter.ExecuteWithFilter(
                    () =>
                    {
                        actionToExecute();
                        result = GuardResult.Success;
                    },
                    e =>
                    {
                        processor.OnException(e, false);
                        result = GuardResult.Failure;
                    });

                return result;
            }
        }
    }
}

