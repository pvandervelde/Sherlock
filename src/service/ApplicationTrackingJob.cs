//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Sherlock.Service
{
    /// <summary>
    /// Defines a 'job' that links all the active dataset processes to the
    /// current process so that all the processes may be terminated at the same
    /// time.
    /// </summary>
    internal sealed class ApplicationTrackingJob
    {
        private enum JobObjectInfoType
        {
            ExtendedLimitInformation = 9
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JobObjectBasicLimitInformation
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public short LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public short ActiveProcessLimit;
            public long Affinity;
            public short PriorityClass;
            public short SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IoCounters
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JobObjectExtendedLimitInformation
        {
            public JobObjectBasicLimitInformation BasicLimitInformation;
            public IoCounters IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        /// <summary>
        /// Defines the constant that indicates that child processes of child
        /// processes will be broken away from the job.
        /// </summary>
        private const short JobObjectLimitSilentBreakawayOk = 0x00001000;

        /// <summary>
        /// Defines the constant that indicates that child processes need to be killed
        /// when the main job is terminated.
        /// </summary>
        private const short JobObjectLimitKillOnJobClose = 0x00002000;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern IntPtr CreateJobObject(
            IntPtr jobAttributes,
            string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern bool SetInformationJobObject(
            IntPtr hJob,
            JobObjectInfoType infoType,
            IntPtr lpJobObjectInfo,
            uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// The handle for the job that links all the child processes to the current
        /// process.
        /// </summary>
        private IntPtr m_Handle;

        /// <summary>
        /// The flag that indicates if the object has been disposed or not.
        /// </summary>
        private volatile bool m_IsDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationTrackingJob"/> class.
        /// </summary>
        public ApplicationTrackingJob()
        {
            m_Handle = CreateJobObject(IntPtr.Zero, null);
            if (m_Handle == null)
            {
                throw new UnableToCreateJobException();
            }

            var info = new JobObjectBasicLimitInformation
                {
                    LimitFlags = JobObjectLimitSilentBreakawayOk | JobObjectLimitKillOnJobClose
                };

            var extendedInfo = new JobObjectExtendedLimitInformation
                {
                    BasicLimitInformation = info
                };

            int length = Marshal.SizeOf(typeof(JobObjectExtendedLimitInformation));
            var extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!SetInformationJobObject(
                m_Handle,
                JobObjectInfoType.ExtendedLimitInformation,
                extendedInfoPtr,
                (uint)length))
            {
                var error = Marshal.GetLastWin32Error();
                throw new UnableToSetJobException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to set information.  Error: {0}",
                        error));
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ApplicationTrackingJob"/> class.
        /// </summary>
        ~ApplicationTrackingJob()
        {
            CleanUpResources();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanUpResources();
            GC.SuppressFinalize(this);
        }

        private void CleanUpResources()
        {
            if (m_IsDisposed)
            {
                return;
            }

            Close();
            m_IsDisposed = true;
        }

        /// <summary>
        /// Closes the job.
        /// </summary>
        public void Close()
        {
            CloseHandle(m_Handle);
            m_Handle = IntPtr.Zero;
        }

        /// <summary>
        /// Links a new child process to the current job.
        /// </summary>
        /// <param name="processToAdd">The child process that should be added.</param>
        public void LinkChildProcessToJob(Process processToAdd)
        {
            if (!AssignProcessToJobObject(m_Handle, processToAdd.Handle))
            {
                throw new UnableToLinkChildProcessToJobException();
            }
        }
    }
}
