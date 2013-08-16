//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores log strings from one of the
    /// tools used in the regression test process.
    /// </summary>
    public sealed class ProcessOutputEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessOutputEventArgs"/> class.
        /// </summary>
        /// <param name="output">The output string.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="output"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="output"/> is empty.
        /// </exception>
        public ProcessOutputEventArgs(string output)
        {
            {
                Lokad.Enforce.Argument(() => output);
                Lokad.Enforce.Argument(() => output, Lokad.Rules.StringIs.NotEmpty);
            }

            Output = output;
        }

        /// <summary>
        /// Gets the output.
        /// </summary>
        public string Output
        {
            get;
            private set;
        }
    }
}
