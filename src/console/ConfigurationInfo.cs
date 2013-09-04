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
        /// <param name="test">The test that was stored in the configuration.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="test"/> is <see langword="null" />.
        /// </exception>
        public ConfigurationInfo(TestDescription test)
        {
            {
                Lokad.Enforce.Argument(() => test);
            }

            Test = test;
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
