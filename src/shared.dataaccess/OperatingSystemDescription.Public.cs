//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API for the operating system description.
    /// </content>
    public sealed partial class OperatingSystemDescription
    {
        /// <summary>
        /// Gets or sets a value indicating whether the object is currently undergoing patching, i.e.
        /// adding all values from the database.
        /// </summary>
        internal bool IsPatching
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object has been patched, i.e. all values have been
        /// extracted from the database, or not.
        /// </summary>
        internal bool IsPatched
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID of the operating system.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_OperatingSystemId;
            }

            set
            {
                pk_OperatingSystemId = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current operating system is used.
        /// </summary>
        public bool IsUsed
        {
            get
            {
                return (Machines != null) && Machines.Any();
            }
        }

        /// <summary>
        /// Gets or sets the pointer size for the operating system.
        /// </summary>
        public OperatingSystemPointerSize PointerSize
        {
            get
            {
                return (OperatingSystemPointerSize)Enum.ToObject(typeof(OperatingSystemPointerSize), ArchitecturePointerSize);
            }

            set
            {
                ArchitecturePointerSize = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the culture of the operating system.
        /// </summary>
        public CultureInfo CultureInfo
        {
            get
            {
                return (Culture != null) ? new CultureInfo(Culture) : CultureInfo.InvariantCulture;
            }

            set
            {
                Culture = value.Name;
            }
        }
    }
}
