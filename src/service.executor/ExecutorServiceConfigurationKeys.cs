//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Configuration;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Defines all the configuration keys for the master service.
    /// </summary>
    internal static class ExecutorServiceConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the amount of time between keep-alive checks.
        /// </summary>
        public static readonly ConfigurationKey KeepAliveCycleTimeInMilliSeconds
            = new ConfigurationKey("KeepAliveCycleTimeInMilliSeconds", typeof(int));

        /// <summary>
        /// The configuration key that is used to retrieve the directory path to the directory that contains the test files.
        /// </summary>
        public static readonly ConfigurationKey TestDataDirectory
            = new ConfigurationKey("TestDataDirectory", typeof(string));

        /// <summary>
        /// The configuration key that is used to retrieve the amount of time that the service waits for the executor application
        /// to start (int).
        /// </summary>
        public static readonly ConfigurationKey ExecutorStartupTimeoutInMilliSeconds
            = new ConfigurationKey("ExecutorStartupTimeoutInMilliSeconds", typeof(int));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    KeepAliveCycleTimeInMilliSeconds,
                    TestDataDirectory,
                    ExecutorStartupTimeoutInMilliSeconds,
                };
        }
    }
}
