//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Stores the values of a parameter that is used in a <see cref="TestStep"/>.
    /// </summary>
    [Serializable]
    public sealed class InputParameter
    {
        /// <summary>
        /// The key for the parameter.
        /// </summary>
        private readonly string m_Key;
        
        /// <summary>
        /// The value for the parameter.
        /// </summary>
        private readonly string m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputParameter"/> class.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="key"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="key"/> is an empty string.
        /// </exception>
        public InputParameter(string key, string value)
        {
            {
                Lokad.Enforce.Argument(() => key);
                Lokad.Enforce.Argument(() => key, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Key = key;
            m_Value = value;
        }

        /// <summary>
        /// Gets the key for the parameter.
        /// </summary>
        public string Key
        {
            get
            {
                return m_Key;
            }
        }

        /// <summary>
        /// Gets the value for the parameter.
        /// </summary>
        public string Value
        {
            get
            {
                return m_Value;
            }
        }
    }
}
