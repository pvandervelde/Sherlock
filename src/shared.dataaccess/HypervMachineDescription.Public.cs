//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API for the Hyper-V machine class.
    /// </content>
    public sealed partial class HypervMachineDescription
    {
        /// <summary>
        /// Gets or sets the ID of the host machine.
        /// </summary>
        public string HostMachineId
        {
            get
            {
                return fk_HostId;
            }

            set
            {
                fk_HostId = value;
            }
        }
    }
}
