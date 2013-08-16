//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the different state changes that the Hyper-V virtual machine can be put in.
    /// </summary>
    internal enum HypervVirtualMachineStateChange
    {
        /// <summary>
        /// Turns the VM on.
        /// </summary>
        Start = 2,

        /// <summary>
        /// Turns the VM off.
        /// </summary>
        Terminate = 3,

        /// <summary>
        /// A hard reset of the VM.
        /// </summary>
        Reboot = 10,
        
        /// <summary>
        /// For future use.
        /// </summary>
        Reset = 11,
        
        /// <summary>
        /// Pauses the VM.
        /// </summary>
        Paused = 32768,

        /// <summary>
        /// Pauses the VM.
        /// </summary>
        Suspended = 32769,
    }
}
