//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores the configuration information for a given test.
    /// </summary>
    internal sealed class ConfigurationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationInfo"/> class.
        /// </summary>
        /// <param name="serverUrl">The URL on which the test server can be found.</param>
        /// <param name="test">The test that was stored in the configuration.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="serverUrl"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="serverUrl"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="test"/> is <see langword="null" />.
        /// </exception>
        public ConfigurationInfo(string serverUrl, TestDescription test)
        {
            {
                Lokad.Enforce.Argument(() => serverUrl);
                Lokad.Enforce.Argument(() => serverUrl, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => test);
            }

            ServerUrl = serverUrl;
            Test = test;
        }

        /// <summary>
        /// Gets the URL for the test server.
        /// </summary>
        public string ServerUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the test that was stored in the configuration.
        /// </summary>
        public TestDescription Test
        {
            get;
            private set;
        }
    }
}
