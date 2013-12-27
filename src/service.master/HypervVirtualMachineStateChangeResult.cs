//----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// The return codes for the state change of the virtual machine.
    /// </summary>
    internal enum HypervVirtualMachineStateChangeResult
    {
        /// <summary>
        /// There were no errors during the state change.
        /// </summary>
        None = 0,

        /// <summary>
        /// The transition to the requested state has started.
        /// </summary>
        TransitionStarted = 4096,

        /// <summary>
        /// The state change failed.
        /// </summary>
        Failed = 32768,

        /// <summary>
        /// The state change was denied.
        /// </summary>
        AccessDenied = 32769,

        /// <summary>
        /// The state change is not supported.
        /// </summary>
        NotSupported = 32770,

        /// <summary>
        /// The current state is unknown.
        /// </summary>
        StatusIsUnknown = 32771,

        /// <summary>
        /// The state change timed out.
        /// </summary>
        Timeout = 32772,

        /// <summary>
        /// The state change parameters are invalid.
        /// </summary>
        InvalidParameter = 32773,

        /// <summary>
        /// The virtual machine is in use.
        /// </summary>
        SystemIsInUse = 32774,

        /// <summary>
        /// The requested state is invalid for the current request.
        /// </summary>
        InvalidStateForThisOperation = 32775,

        /// <summary>
        /// The provided data is of an incorrect type.
        /// </summary>
        IncorrectDataType = 32776,

        /// <summary>
        /// The system is not available for the state change.
        /// </summary>
        SystemIsNotAvailable = 32777,

        /// <summary>
        /// The system is out of memory.
        /// </summary>
        OutOfMemory = 32778,
    }
}
