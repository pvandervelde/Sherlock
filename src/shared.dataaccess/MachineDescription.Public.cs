//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API for the machine description.
    /// </content>
    public partial class MachineDescription
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
        /// Gets or sets the ID of the machine.
        /// </summary>
        public string Id
        {
            get
            {
                return pk_MachineId;
            }

            set
            {
                pk_MachineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the operating system related to this machine.
        /// </summary>
        public int OperatingSystemId
        {
            get
            {
                return fk_OperatingSystem;
            }

            set
            {
                fk_OperatingSystem = value;
            }
        }
    }
}
