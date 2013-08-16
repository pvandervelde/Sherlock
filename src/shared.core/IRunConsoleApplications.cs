//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that run console applications.
    /// </summary>
    public interface IRunConsoleApplications
    {
        /// <summary>
        /// Runs the specified executable.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The exit code of the application that was executed.</returns>
        int Run(string executablePath, string[] arguments);

        /// <summary>
        /// Occurs when new console output is available.
        /// </summary>
        event EventHandler<ProcessOutputEventArgs> OnConsoleOutput;
    }
}
