//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines a set of configuration keys that are shared between all the different applications.
    /// </summary>
    public static class SharedConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the amount of time the system will wait for a 
        /// positive response to a network ping command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable.")]
        public static readonly ConfigurationKey PingTimeoutInMilliseconds
            = new ConfigurationKey("PingTimeoutInMilliseconds", typeof(int));

        /// <summary>
        /// The configuration key that is used to retrieve the amount of time the system will wait for
        /// a sign-on to the network to succeed.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable.")]
        public static readonly ConfigurationKey MaximumNetworkSignInTimeInMilliseconds
            = new ConfigurationKey("MaximumNetworkSignInTimeInMilliseconds", typeof(int));

        /// <summary>
        /// The configuration key that is used to retrieve the amount of time the system will wait between
        /// two consecutive ping operations.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable.")]
        public static readonly ConfigurationKey PingCycleTimeInMilliseconds
            = new ConfigurationKey("PingCycleTimeInMilliseconds", typeof(int));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    PingTimeoutInMilliseconds,
                    MaximumNetworkSignInTimeInMilliseconds,
                    PingCycleTimeInMilliseconds,
                };
        }
    }
}
