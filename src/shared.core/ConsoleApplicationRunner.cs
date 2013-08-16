//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;
using Sherlock.Shared.Core.Properties;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Runs applications on the console or command line.
    /// </summary>
    public sealed class ConsoleApplicationRunner : IRunConsoleApplications
    {
        /// <summary>
        /// The number of bytes per KiloByte.
        /// </summary>
        private const int BytesPerKb = 1024;

        private static ProcessStartInfo CreateStartInfo(string executablePath, string[] arguments)
        {
            var startInfo = new ProcessStartInfo();
            {
                startInfo.FileName = executablePath;

                // Build the command line arguments
                startInfo.Arguments = string.Join(" ", arguments);

                // do not display an error dialog if the process
                // can't be started
                startInfo.ErrorDialog = false;

                // Do not use the system shell. We want to
                // be able to redirect the output streams
                startInfo.UseShellExecute = false;

                // Redirect the standard output / error streams
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
            }

            return startInfo;
        }

        private readonly ProcessInformation m_ProcessInformation = new ProcessInformation();

        /// <summary>
        /// Runs the specified executable.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The exit code of the application that was executed.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Not sure what to catch here.")]
        public int Run(string executablePath, string[] arguments)
        {
            m_ProcessInformation.Clear();

            // Log the command line for debugging purposes.
            {
                var text = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.ConsoleApplicationRunner_Output_RunningExe_WithPathAndArguments,
                    executablePath,
                    string.Join(" ", arguments));
                RaiseOnConsoleOutput(text);
            }

            try
            {
                // Start the console app?
                var startInfo = CreateStartInfo(executablePath, arguments);
                using (var exec = new Process())
                {
                    // Define the application we want to start
                    exec.StartInfo = startInfo;

                    // Redirect the output and error stream. Note that
                    // we need to be careful how to read from these streams
                    // in order to prevent deadlocks from happening
                    // see e.g. here: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.errordatareceived.aspx
                    exec.ErrorDataReceived += (s, e) =>
                    {
                        // There is no reason to get a lock
                        // before setting the value because
                        // we wait for the process to exit before
                        // checking the value. So there is no
                        // simultaneous access at any point.
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            var output = string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.ConsoleApplicationRunner_ErrorWhileRunning_WithError, 
                                e.Data);
                            RaiseOnConsoleOutput(output);
                        }
                    };
                    exec.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            var output = string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.ConsoleApplicationRunner_OutputWhileRunning_WithOutput, 
                                e.Data);
                            RaiseOnConsoleOutput(output);
                        }
                    };

                    // Start the process
                    exec.Start();
                    exec.BeginErrorReadLine();
                    exec.BeginOutputReadLine();

                    // Wait for the process to exit
                    while (!exec.HasExited)
                    {
                        // Log the statistics for the process
                        TrackProcessStatistics(exec);

                        // Sleep for 1 second before polling again
                        Thread.Sleep(1000);
                    }

                    // Notify the user that the process has exited.
                    {
                        var text = string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.ConsoleApplicationRunner_Output_ProcessCompleted_WithTimeAndProcessStats,
                            m_ProcessInformation.TotalTime,
                            StoreStatistics(exec));
                        RaiseOnConsoleOutput(text);
                    }

                    return exec.ExitCode;
                }
            }
            catch (Exception e)
            {
                var log = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.ConsoleApplicationRunner_Error_FailedToRunExe_WithPathAndError,
                    executablePath,
                    e);
                RaiseOnConsoleOutput(log);

                return -1;
            }
        }

        /// <summary>
        /// Logs the CPU, time and memory statics for the process.
        /// </summary>
        /// <param name="exec">The process for which the statistics should be logged.</param>
        private void TrackProcessStatistics(Process exec)
        {
            var pi = m_ProcessInformation;
            if (exec.PrivateMemorySize64 > pi.PrivateMemorySize)
            {
                pi.PrivateMemorySize = exec.PrivateMemorySize64;
            }

            if (exec.VirtualMemorySize64 > pi.VirtualMemorySize)
            {
                pi.VirtualMemorySize = exec.VirtualMemorySize64;
            }

            if (exec.WorkingSet64 > pi.WorkingSet)
            {
                pi.WorkingSet = exec.WorkingSet64;
            }

            if (exec.PeakPagedMemorySize64 > pi.PeakPagedMemorySize)
            {
                pi.PeakPagedMemorySize = exec.PeakPagedMemorySize64;
            }

            if (exec.PeakVirtualMemorySize64 > pi.PeakVirtualMemorySize)
            {
                pi.PeakVirtualMemorySize = exec.PeakVirtualMemorySize64;
            }

            if (exec.PeakWorkingSet64 > pi.PeakWorkingSet)
            {
                pi.PeakWorkingSet = exec.PeakWorkingSet64;
            }

            if (exec.NonpagedSystemMemorySize64 > pi.NonPagedSystemMemorySize)
            {
                pi.NonPagedSystemMemorySize = exec.NonpagedSystemMemorySize64;
            }

            if (exec.PagedSystemMemorySize64 > pi.PagedSystemMemorySize)
            {
                pi.PagedSystemMemorySize = exec.PagedSystemMemorySize64;
            }

            if (exec.PagedMemorySize64 > pi.PagedMemorySize)
            {
                pi.PagedMemorySize = exec.PagedMemorySize64;
            }

            pi.PrivilegedProcessorTime = exec.PrivilegedProcessorTime;
            pi.UserProcessorTime = exec.UserProcessorTime;
            pi.TotalProcessorTime = exec.TotalProcessorTime;

            pi.StartTime = exec.StartTime;
        }

        /// <summary>
        /// Logs the statistics for the process that was run.
        /// </summary>
        /// <param name="exec">The process for which the statistics have to be collected.</param>
        /// <returns>
        /// A string containing a summary of the statistics for the process.
        /// </returns>
        private string StoreStatistics(Process exec)
        {
            m_ProcessInformation.ExitTime = exec.ExitTime;
            var builder = new StringBuilder();
            {
                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForFilePath,
                        exec.StartInfo.FileName));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForStartTime,
                        m_ProcessInformation.StartTime));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForEndTime,
                        m_ProcessInformation.ExitTime));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForTotalTime,
                        m_ProcessInformation.TotalTime));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPrivilegedCpuTime,
                        m_ProcessInformation.PrivilegedProcessorTime));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForUserCpuTime,
                        m_ProcessInformation.UserProcessorTime));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForTotalCpuTime,
                        m_ProcessInformation.TotalProcessorTime));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPrivateMemory,
                        m_ProcessInformation.PrivateMemorySize / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForVirtualMemory,
                        m_ProcessInformation.VirtualMemorySize / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForWorkingSetMemory,
                        m_ProcessInformation.WorkingSet / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPeakPagedMemory,
                        m_ProcessInformation.PeakPagedMemorySize / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPeakVirtualMemory,
                        m_ProcessInformation.PeakVirtualMemorySize / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPeakWorkingSetMemory,
                        m_ProcessInformation.PeakWorkingSet / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForNonPagedSystemMemory,
                        m_ProcessInformation.NonPagedSystemMemorySize / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPagedSystemMemory,
                        m_ProcessInformation.PagedSystemMemorySize / BytesPerKb));

                builder.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.ConsoleApplicationRunner_ProcessStats_ForPagedSystemMemory,
                        m_ProcessInformation.PagedMemorySize / BytesPerKb));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Occurs when new console output is available.
        /// </summary>
        public event EventHandler<ProcessOutputEventArgs> OnConsoleOutput;

        /// <summary>
        /// Raises the console output event.
        /// </summary>
        /// <param name="output">The output.</param>
        private void RaiseOnConsoleOutput(string output)
        {
            EventHandler<ProcessOutputEventArgs> local = OnConsoleOutput;
            if (local != null)
            {
                local(this, new ProcessOutputEventArgs(output));
            }
        }
    }
}
