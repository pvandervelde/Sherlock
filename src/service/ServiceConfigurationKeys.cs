//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Configuration;

namespace Sherlock.Service
{
    /// <summary>
    /// Defines all the configuration keys for the service.
    /// </summary>
    internal static class ServiceConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the amount of time between keep-alive checks.
        /// </summary>
        public static readonly ConfigurationKey KeepAliveCycleTimeInMilliSeconds
            = new ConfigurationKey("KeepAliveCycleTimeInMilliSeconds", typeof(int));

        /// <summary>
        /// The configuration key that is used to retrieve the amount of time between update checks.
        /// </summary>
        public static readonly ConfigurationKey UpdateCycleTimeInMilliseconds
            = new ConfigurationKey("UpdateCycleTimeInMilliseconds", typeof(int));

        /// <summary>
        /// The configuration key that is used to retrieve the URL pointing to the update manifest.
        /// </summary>
        public static readonly ConfigurationKey UpdateManifestUri
            = new ConfigurationKey("UpdateManifestUri", typeof(string));

        /// <summary>
        /// The configuration key that is used to retrieve the name of the application.
        /// </summary>
        public static readonly ConfigurationKey ApplicationName
            = new ConfigurationKey("ApplicationName", typeof(string));

        /// <summary>
        /// The configuration key that is used to retrieve the directory in which the application is placed.
        /// </summary>
        public static readonly ConfigurationKey ApplicationDirectory
            = new ConfigurationKey("ApplicationDirectory", typeof(string));

        /// <summary>
        /// The configuration key that is used to retrieve the directory in which the application is placed.
        /// </summary>
        public static readonly ConfigurationKey ManifestPublicKeyFile
            = new ConfigurationKey("ManifestPublicKeyFile", typeof(string));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    KeepAliveCycleTimeInMilliSeconds,
                    UpdateCycleTimeInMilliseconds,
                    UpdateManifestUri,
                    ApplicationName,
                    ApplicationDirectory,
                    ManifestPublicKeyFile,
                };
        }
    }
}
