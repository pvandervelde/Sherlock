//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Provides the possible return codes for a WMI method call.
    /// </summary>
    internal enum WmiReturnCode
    {
        /// <summary>
        /// The call was completed successfully.
        /// </summary>
        Completed = 0,
        
        /// <summary>
        /// The method call was started asynchronously, but was not completed yet.
        /// </summary>
        Started = 4096,

        /// <summary>
        /// The method call failed.
        /// </summary>
        Failed = 32768,

        /// <summary>
        /// An access denied error occurred.
        /// </summary>
        AccessDenied = 32769,

        /// <summary>
        /// The method call was not supported by the management object.
        /// </summary>
        NotSupported = 32770,

        /// <summary>
        /// The method returned an unknown code.
        /// </summary>
        Unknown = 32771,

        /// <summary>
        /// The method timed-out.
        /// </summary>
        Timeout = 32772,

        /// <summary>
        /// An invalid parameter was provided.
        /// </summary>
        InvalidParameter = 32773,

        /// <summary>
        /// The system is in use.
        /// </summary>
        SystemInUse = 32774,

        /// <summary>
        /// The system is in an invalid state.
        /// </summary>
        InvalidState = 32775,

        /// <summary>
        /// An incorrect data type was provided.
        /// </summary>
        IncorrectDataType = 32776,

        /// <summary>
        /// The system is not available.
        /// </summary>
        SystemNotAvailable = 32777,

        /// <summary>
        /// The system is out of memory.
        /// </summary>
        OutofMemory = 32778,
    }
}
