//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sherlock.Console.Nuclei.ExceptionHandling
{
    /// <summary>
    /// An exception handler.
    /// </summary>
    /// <design>
    /// This class must be public because we use it in the AppDomainBuilder.
    /// </design>
    [Serializable]
    public sealed class ExceptionHandler : IExceptionHandler, IDisposable
    {
        /// <summary>
        /// The collection of loggers that must be notified if an exception happens.
        /// </summary>
        private readonly IExceptionProcessor[] m_Loggers;

        /// <summary>
        /// Indicates if the object has been disposed or not.
        /// </summary>
        private volatile bool m_WasDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="exceptionProcessors">The collection of exception processors that will be used to log any unhandled exception.</param>
        public ExceptionHandler(params IExceptionProcessor[] exceptionProcessors)
        {
            m_Loggers = exceptionProcessors ?? new IExceptionProcessor[0];
        }

        /// <summary>
        /// Used when an unhandled exception occurs in an <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="isApplicationTerminating">Indicates if the application is about to shut down or not.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're doing exception handling here, we don't really want anything to escape.")]
        public void OnException(Exception exception, bool isApplicationTerminating)
        {
            if (m_WasDisposed)
            {
                return;
            }

            // Something has gone really wrong here. We need to be very careful
            // when we try to deal with this exception because:
            // - We might be here due to assembly loading issues, so we can't load
            //   any code which is not in the current class or in one of the system
            //   assemblies (that is we assume any code in the GAC is available ...
            //   which obviously may be incorrect).
            // - We might be here because the CLR failed hard (e.g. OutOfMemoryException
            //   and friends). In this case we're toast. We'll try our normal approach
            //   but that will probably fail ...
            //
            // We don't want to throw an exception if we're handling unhandled exceptions ...
            foreach (var logger in m_Loggers)
            {
                try
                {
                    logger.Process(exception);
                }
                catch (Exception)
                {
                    // Stuffed. Just give up.
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_WasDisposed)
            {
                return;
            }

            foreach (var logger in m_Loggers)
            {
                logger.Dispose();
            }

            m_WasDisposed = true;
        }
    }
}

