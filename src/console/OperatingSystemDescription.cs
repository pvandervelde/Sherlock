//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores the description of an operating system.
    /// </summary>
    internal sealed class OperatingSystemDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemDescription"/> class.
        /// </summary>
        /// <param name="name">The name of the operating system.</param>
        /// <param name="servicePack">The service pack of the operating system.</param>
        /// <param name="culture">The culture of the operating system.</param>
        /// <param name="pointerSize">The pointer size, either 32-bits or 64-bits.</param>
        public OperatingSystemDescription(string name, string servicePack, CultureInfo culture, int pointerSize)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(name), "The name should not be an empty string.");
                Debug.Assert(servicePack != null, "The service pack should not be a null reference.");
                Debug.Assert(culture != null, "The culture should not be a null reference.");
            }

            Name = name;
            ServicePack = servicePack;
            Culture = culture;
            PointerSize = pointerSize;
        }

        /// <summary>
        /// Gets the name of the operating system.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service pack of the operating system.
        /// </summary>
        public string ServicePack
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the culture of the operating system.
        /// </summary>
        public CultureInfo Culture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pointer size of the operating system, either 32-bits or 64-bits.
        /// </summary>
        public int PointerSize
        {
            get;
            private set;
        }
    }
}
