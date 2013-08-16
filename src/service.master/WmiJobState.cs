//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the state of a WMI job.
    /// </summary>
    internal enum WmiJobState
    {
        /// <summary>
        /// The job is new.
        /// </summary>
        New = 2,

        /// <summary>
        /// The job is starting.
        /// </summary>
        Starting = 3,

        /// <summary>
        /// The job is running.
        /// </summary>
        Running = 4,

        /// <summary>
        /// The job has been suspended.
        /// </summary>
        Suspended = 5,

        /// <summary>
        /// The job is shutting down.
        /// </summary>
        ShuttingDown = 6,

        /// <summary>
        /// The job has been completed.
        /// </summary>
        Completed = 7,

        /// <summary>
        /// The job has been terminated.
        /// </summary>
        Terminated = 8,

        /// <summary>
        /// The job has been killed.
        /// </summary>
        Killed = 9,

        /// <summary>
        /// The job experienced an exception.
        /// </summary>
        Exception = 10,

        /// <summary>
        /// The job is a service.
        /// </summary>
        Service = 11,
    }
}
