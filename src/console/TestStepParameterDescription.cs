//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores the key and value for a test step parameter.
    /// </summary>
    internal sealed class TestStepParameterDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepParameterDescription"/> class.
        /// </summary>
        /// <param name="key">The key for the parameter.</param>
        /// <param name="value">The value for the parameter.</param>
        public TestStepParameterDescription(string key, string value)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(key), "The key of a parameter should not be an empty string.");
                Debug.Assert(!string.IsNullOrEmpty(value), "The value of a parameter should not be an empty string.");
            }

            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the key for the parameter.
        /// </summary>
        public string Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value for the parameter.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
    }
}
