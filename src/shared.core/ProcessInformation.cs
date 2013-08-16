//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Stores information about a process that was run by an
    /// <see cref="IRunConsoleApplications"/> object.
    /// </summary>
    internal sealed class ProcessInformation
    {
        /// <summary>
        /// Gets or sets the size of the private memory.
        /// </summary>
        public long PrivateMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the virtual memory.
        /// </summary>
        public long VirtualMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the working set.
        /// </summary>
        public long WorkingSet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the peak paged memory.
        /// </summary>
        public long PeakPagedMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the peak virtual memory.
        /// </summary>
        public long PeakVirtualMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the peak working set.
        /// </summary>
        public long PeakWorkingSet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the non-paged system memory.
        /// </summary>
        public long NonPagedSystemMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the paged system memory.
        /// </summary>
        public long PagedSystemMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the paged memory.
        /// </summary>
        public long PagedMemorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the privileged processor time.
        /// </summary>
        public TimeSpan PrivilegedProcessorTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user processor time.
        /// </summary>
        public TimeSpan UserProcessorTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total processor time.
        /// </summary>
        public TimeSpan TotalProcessorTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time the process started.
        /// </summary>
        public DateTimeOffset StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time the process exited.
        /// </summary>
        public DateTimeOffset ExitTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total time that the process ran.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                return ExitTime - StartTime;
            }
        }

        /// <summary>
        /// Clears the stored data.
        /// </summary>
        public void Clear()
        {
            PrivateMemorySize = 0;
            VirtualMemorySize = 0;
            WorkingSet = 0;

            PeakPagedMemorySize = 0;
            PeakVirtualMemorySize = 0;
            PeakWorkingSet = 0;

            NonPagedSystemMemorySize = 0;
            PagedSystemMemorySize = 0;
            PagedMemorySize = 0;

            PrivilegedProcessorTime = TimeSpan.Zero;
            UserProcessorTime = TimeSpan.Zero;
            TotalProcessorTime = TimeSpan.Zero;

            StartTime = DateTimeOffset.MinValue;
            ExitTime = DateTimeOffset.MinValue;
        }
    }
}
