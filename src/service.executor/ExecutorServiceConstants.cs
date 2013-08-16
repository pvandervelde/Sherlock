//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Defines the constants for the executor application.
    /// </summary>
    internal static class ExecutorServiceConstants
    {
        /// <summary>
        /// The name of the configuration section that describes the configuration of the application.
        /// </summary>
        public const string ConfigurationSectionApplicationSettings = "executorservice";

        /// <summary>
        /// The prefix used for each log message.
        /// </summary>
        public const string LogPrefix = "Sherlock.Service.Executor";

        /// <summary>
        /// The default amount of time the service will wait for the executor application to start 
        /// and connect.
        /// </summary>
        public const int DefaultExecutorStartupTimeoutInMilliSeconds = 60 * 1000;
    }
}
