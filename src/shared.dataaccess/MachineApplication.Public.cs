//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API for the machine-application description.
    /// </content>
    public partial class MachineApplication
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
        /// Gets the ID of the machine application.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_MachineApplicationId;
            }
        }

        /// <summary>
        /// Gets the ID of the machine.
        /// </summary>
        public string MachineId
        {
            get
            {
                return fk_MachineId;
            }
        }

        /// <summary>
        /// Gets the ID of the application.
        /// </summary>
        public int ApplicationId
        {
            get
            {
                return fk_ApplicationId;
            }
        }
    }
}
