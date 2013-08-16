//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the different states a Hyper-V virtual machine can be in.
    /// </summary>
    internal enum HypervVirtualMachineState
    {
        /// <summary>
        /// The state of the VM could not be determined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The VM is running.
        /// </summary>
        Running = 2,

        /// <summary>
        /// The VM is turned off.
        /// </summary>
        TurnedOff = 3,
        
        /// <summary>
        /// The VM is paused.
        /// </summary>
        Paused = 32768,

        /// <summary>
        /// The VM is in a saved state.
        /// </summary>
        Suspended = 32769,

        /// <summary>
        /// The VM is starting. This is a transitional state between 3 (TurnedOff) or 32769 (Suspended) and 2 (Running) initiated by a call to the 
        /// RequestStateChange method with a RequestedState parameter of 2 (Enabled).
        /// </summary>
        Starting = 32770,

        /// <summary>
        /// Starting with Windows Server 2008 R2 this value is not supported. If the VM is performing a snapshot operation, the element at name 1
        /// of the OperationalStatus property array will contain 32768 (Creating Snapshot), 32769 (Applying Snapshot), or 32770 (Deleting Snapshot).
        /// </summary>
        Snapshotting = 32771,

        /// <summary>
        /// The VM is saving its state. This is a transitional state between 2 (Running) and 32769 (Suspended) initiated by a call to the 
        /// RequestStateChange method with a RequestedState parameter of 32769 (Suspended).
        /// </summary>
        Saving = 32773,

        /// <summary>
        /// The VM is turning off. This is a transitional state between 2 (Running) and 3 (TurnedOff) initiated by a call to the 
        /// RequestStateChange method with a RequestedState parameter of 3 (Disabled) or a guest operating system initiated power off.
        /// </summary>
        Stopping = 32774,

        /// <summary>
        /// The VM is pausing. This is a transitional state between 2 (Running) and 32768 (Paused) initiated by a call to the RequestStateChange
        ///  method with a RequestedState parameter of 32768 (Paused).
        /// </summary>
        Pausing = 32776,

        /// <summary>
        /// The VM is resuming from a paused state. This is a transitional state between 32768 (Paused) and 2 (Running).
        /// </summary>
        Resuming = 32777,
    }
}
