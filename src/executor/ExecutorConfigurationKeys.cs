//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Configuration;

namespace Sherlock.Executor
{
    /// <summary>
    /// Defines the configuration keys for the executor application.
    /// </summary>
    internal static class ExecutorConfigurationKeys
    {
        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>();
        }
    }
}
