//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Configuration;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the configuration keys for the console application.
    /// </summary>
    internal static class ConsoleConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the web service URL.
        /// </summary>
        public static readonly ConfigurationKey WebServiceUrl
            = new ConfigurationKey("WebServiceUrl", typeof(string));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    WebServiceUrl,
                };
        }
    }
}
