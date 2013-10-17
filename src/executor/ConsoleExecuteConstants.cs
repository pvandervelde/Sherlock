//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using Sherlock.Shared.Core;

namespace Sherlock.Executor
{
    /// <summary>
    /// Defines constants for console application execution steps.
    /// </summary>
    internal static class ConsoleExecuteConstants
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "executor.info.log";

        /// <summary>
        /// The prefix used for each log message.
        /// </summary>
        public const string LogPrefix = "Sherlock.Executor-Console";

        /// <summary>
        /// Returns the path to the log file for the application.
        /// </summary>
        /// <returns>The full path to the log file for the application.</returns>
        public static string LogPath()
        {
            return Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultInfoFileName);
        }
    }
}
