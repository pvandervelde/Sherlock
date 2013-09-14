//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Autofac;
using Ionic.Zip;
using NAdoni;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Properties;
using Sherlock.Shared.Core;

namespace Sherlock.Service
{
    /// <summary>
    /// Defines the methods used by the service manager to control the service.
    /// </summary>
    internal sealed class ServiceEntryPoint : IDisposable
    {
        /// <summary>
        /// The object that links the application process to the current process so that application will be stopped
        /// if the current process terminates.
        /// </summary>
        private static readonly ApplicationTrackingJob s_TrackingJob
            = new ApplicationTrackingJob();

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The object used to lock on for update checks.
        /// </summary>
        private readonly object m_UpdateLock = new object();

        /// <summary>
        /// The timer that is used to periodically check that all the test environments are still running.
        /// </summary>
        private readonly System.Timers.Timer m_Timer = new System.Timers.Timer();

        /// <summary>
        /// The IOC container for the service.
        /// </summary>
        private IContainer m_Container;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The next point in time where a check for updates should be run.
        /// </summary>
        private DateTimeOffset m_NextTimeToCheckForUpdates;

        /// <summary>
        /// The minimum amount of time between update checks.
        /// </summary>
        private TimeSpan m_UpdateCheckCycleTime;

        /// <summary>
        /// The object that handles the updating of the actual service code.
        /// </summary>
        private Updater m_Updater;

        /// <summary>
        /// The URL to the update manifest.
        /// </summary>
        private Uri m_ManifestUri;

        /// <summary>
        /// The XML text that contains the public key that is used to verify that the update manifest is correct.
        /// </summary>
        private string m_PublicKeyXml;

        /// <summary>
        /// The full path to the location where the application is stored.
        /// </summary>
        private string m_ApplicationDirectory;

        /// <summary>
        /// The name of the application.
        /// </summary>
        private string m_ApplicationName;

        /// <summary>
        /// The current version of the application.
        /// </summary>
        private Version m_ApplicationVersion;

        /// <summary>
        /// The process that is running the actual job.
        /// </summary>
        private Process m_Application;

        /// <summary>
        /// A flag that indicates that the current service is stopping the application.
        /// </summary>
        private bool m_IsStopping;

        /// <summary>
        /// A flag that indicates that the running application is being updated.
        /// </summary>
        private volatile bool m_IsUpdating;

        /// <summary>
        /// A flag that indicates that the application has been stopped.
        /// </summary>
        private volatile bool m_HasBeenStopped;

        /// <summary>
        /// A flag that indicates if the service has been disposed or not.
        /// </summary>
        private volatile bool m_IsDisposed;

        /// <summary>
        /// This is the synchronization point that prevents events
        /// from running concurrently, and prevents the main thread 
        /// from executing code after the Stop method until any 
        /// event handlers are done executing.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private int m_SyncPoint;

        /// <summary>
        /// Processes the elapsed event of the progress timer.
        /// </summary>
        /// <param name="sender">The timer at which the event originated.</param>
        /// <param name="e">The event arguments.</param>
        /// <design>
        /// <para>
        /// This code assumes that overlapping events can be
        /// discarded. That is, if an TimerElapsed event is raised before 
        /// the previous event is finished processing, the second
        /// event is ignored. In this case that is probably not 
        /// directly what we want however we push all the event
        /// processing onto a seperate thread so the execution of
        /// the event should not take very long compared to the 
        /// timer interval.
        /// </para>
        /// </design>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // CompareExchange is used to take control of m_SyncPoint, 
            // and to determine whether the attempt was successful. 
            // CompareExchange attempts to put 1 into syncPoint, but
            // only if the current value of syncPoint is zero 
            // (specified by the third parameter). If another thread
            // has set syncPoint to 1, or if the control thread has
            // set syncPoint to -1, the current event is skipped. 
            if (Interlocked.CompareExchange(ref m_SyncPoint, 1, 0) == 0)
            {
                if (!IsApplicationAlive())
                {
                    Task.Factory.StartNew(StartApplication);
                }

                if (ShouldCheckForUpdates())
                {
                    Task.Factory.StartNew(UpdateApplication);
                }

                // Release control of syncPoint
                // We can just write to the value because integers 
                // are atomically written.
                // 
                // On top of that we only use this variable internally and
                // we'll never do anything with it if the value is unequal to 0.
                m_SyncPoint = 0;
            }
        }

        private bool IsApplicationAlive()
        {
            lock (m_Lock)
            {
                if (!m_IsStopping && !m_IsUpdating)
                {
                    return (m_Application != null) && !m_Application.HasExited;
                }

                return true;
            }
        }

        private void StartApplication()
        {
            lock (m_Lock)
            {
                var filePath = Path.Combine(m_ApplicationDirectory, m_ApplicationName);
                if (!m_IsStopping && !m_IsUpdating && ((m_Application == null) || m_Application.HasExited) && File.Exists(filePath))
                {
                    var processArgs = new ProcessStartInfo
                        {
                            FileName = m_ApplicationName,
                            Arguments = string.Empty,
                            WorkingDirectory = m_ApplicationDirectory,

                            CreateNoWindow = true,
                            ErrorDialog = false,

                            RedirectStandardError = false,
                            RedirectStandardInput = false,
                            RedirectStandardOutput = false,

                            UseShellExecute = true,
                        };

                    m_Application = Process.Start(processArgs);
                    s_TrackingJob.LinkChildProcessToJob(m_Application);

                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        ServiceConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_StartedApplication_WithPath,
                            filePath));
                }
            }
        }

        private bool ShouldCheckForUpdates()
        {
            lock (m_Lock)
            {
                return (!m_IsUpdating) && (DateTimeOffset.Now > m_NextTimeToCheckForUpdates);
            }
        }

        private void UpdateApplication()
        {
            if (m_IsUpdating)
            {
                return;
            }

            m_IsUpdating = true;
            try
            {
                try
                {
                    var update = HasUpdateAsync();
                    update.Wait();

                    var info = update.Result;
                    if (info.UpdateIsAvailableAndValid)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Debug,
                            ServiceConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_DownloadingUpdate_WithInfo,
                                info.ProductName,
                                info.LatestAvailableVersion,
                                info.DownloadLocation));

                        var file = DownloadUpdateAsync(info);
                        var notifyTask = NotifyApplicationOfShutdownAsync();

                        file.Wait();
                        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
                        using (var zipFile = ZipFile.Read(file.Result.FullName))
                        {
                            foreach (var entry in zipFile)
                            {
                                entry.Extract(directory, ExtractExistingFileAction.OverwriteSilently);
                            }
                        }

                        notifyTask.Wait();
                        if (Directory.Exists(m_ApplicationDirectory))
                        {
                            foreach (var element in Directory.GetFiles(m_ApplicationDirectory, "*.*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    File.Delete(element);

                                    m_Diagnostics.Log(
                                        LevelToLog.Debug,
                                        ServiceConstants.LogPrefix,
                                        string.Format(
                                            CultureInfo.InvariantCulture,
                                            Resources.Log_Messages_DeletedProgramFile_WithFile,
                                            element));
                                }
                                catch (IOException e)
                                {
                                    m_Diagnostics.Log(
                                        LevelToLog.Debug,
                                        ServiceConstants.LogPrefix,
                                        string.Format(
                                            CultureInfo.InvariantCulture,
                                            Resources.Log_Messages_FailedToDeleteProgramFile_WithFileAndException,
                                            element,
                                            e));
                                }
                                catch (SecurityException e)
                                {
                                    m_Diagnostics.Log(
                                        LevelToLog.Debug,
                                        ServiceConstants.LogPrefix,
                                        string.Format(
                                            CultureInfo.InvariantCulture,
                                            Resources.Log_Messages_FailedToDeleteProgramFile_WithFileAndException,
                                            element,
                                            e));
                                }
                            }
                        }

                        if (!Directory.Exists(m_ApplicationDirectory))
                        {
                            Directory.CreateDirectory(m_ApplicationDirectory);
                        }

                        foreach (var filePath in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
                        {
                            var destination = Path.Combine(
                                m_ApplicationDirectory, 
                                filePath.Replace(directory, string.Empty).TrimStart(Path.DirectorySeparatorChar));
                            File.Copy(filePath, destination, true);
                        }

                        m_ApplicationVersion = GetApplicationVersion();
                    }

                    lock (m_Lock)
                    {
                        m_NextTimeToCheckForUpdates = DateTimeOffset.Now + m_UpdateCheckCycleTime;
                    }
                }
                catch (Exception e)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        ServiceConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_FailedDownloadApplicationUpdate_WithException,
                            e));
                }
            }
            finally
            {
                m_IsUpdating = false;
            }
        }

        private Version GetApplicationVersion()
        {
            var path = Path.Combine(m_ApplicationDirectory, m_ApplicationName);
            if (!File.Exists(path))
            {
                return new Version(0, 0, 0, 0);
            }

            var fileVersion = FileVersionInfo.GetVersionInfo(path);
            return new Version(fileVersion.FileVersion);
        }

        private Task<UpdateInformation> HasUpdateAsync()
        {
            return Task<UpdateInformation>.Factory.StartNew(
                () =>
                {
                    lock (m_UpdateLock)
                    {
                        var info = m_Updater.MostRecentUpdateOnRemoteServer(m_ManifestUri, m_PublicKeyXml, m_ApplicationVersion);
                        return info;
                    }
                });
        }

        private async Task<FileInfo> DownloadUpdateAsync(UpdateInformation info)
        {
            return await m_Updater.StartDownloadAsync(info);
        }

        private Task NotifyApplicationOfShutdownAsync()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    lock (m_Lock)
                    {
                        if (m_Application == null)
                        {
                            return;
                        }

                        if (!m_Application.HasExited)
                        {
                            m_Application.Kill();
                        }

                        m_Diagnostics.Log(
                            LevelToLog.Info,
                            ServiceConstants.LogPrefix,
                            Resources.Log_Messages_ApplicationTerminated);
                    }
                });
        }

        /// <summary>
        /// Stops the progress timer so that all currently running events
        /// are finished.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private void StopTimer()
        {
            m_Timer.Stop();

            // Ensure that if an event is currently executing,
            // no further processing is done on this thread until
            // the event handler is finished. This is accomplished
            // by using CompareExchange to place -1 in syncPoint,
            // but only if syncPoint is currently zero (specified
            // by the third parameter of CompareExchange). 
            // CompareExchange returns the original value that was
            // in syncPoint. If it was not zero, then there's an
            // event handler running, and it is necessary to try
            // again.
            while (Interlocked.CompareExchange(ref m_SyncPoint, -1, 0) != 0)
            {
                // Give up the rest of this thread's current time
                // slice.
                Thread.Yield();
            }

            // Any processing done after this point does not conflict
            // with timer events. This is the purpose of the call to
            // CompareExchange. If the processing done here would not
            // cause a problem when run concurrently with timer events,
            // then there is no need for the extra synchronization.
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent
        /// to the service by the Service Control Manager (SCM) or when the operating
        /// system starts (for a service that starts automatically). Specifies actions
        /// to take when the service starts.
        /// </summary>
        public void OnStart()
        {
            m_Container = DependencyInjection.CreateContainer();
            m_Diagnostics = m_Container.Resolve<SystemDiagnostics>();

            var configuration = m_Container.Resolve<IConfiguration>();
            if (!configuration.HasValueFor(ServiceConfigurationKeys.ApplicationName))
            {
                throw new MissingConfigurationException();
            }

            if (!configuration.HasValueFor(ServiceConfigurationKeys.UpdateManifestUri))
            {
                throw new MissingConfigurationException();
            }

            var cycleTimeInMilliSeconds = configuration.HasValueFor(ServiceConfigurationKeys.KeepAliveCycleTimeInMilliSeconds)
                ? configuration.Value<int>(ServiceConfigurationKeys.KeepAliveCycleTimeInMilliSeconds)
                : GlobalConstants.DefaultKeepAliveCycleTimeInMilliseconds;

            var updateCycleTime = configuration.HasValueFor(ServiceConfigurationKeys.UpdateCycleTimeInMilliseconds)
                ? configuration.Value<int>(ServiceConfigurationKeys.UpdateCycleTimeInMilliseconds)
                : GlobalConstants.DefaultUpdateCycleTimeInMilliseconds;
            m_UpdateCheckCycleTime = TimeSpan.FromMilliseconds(updateCycleTime);

            m_ApplicationName = configuration.Value<string>(ServiceConfigurationKeys.ApplicationName);
            m_ApplicationDirectory = configuration.HasValueFor(ServiceConfigurationKeys.ApplicationDirectory)
                ? configuration.Value<string>(ServiceConfigurationKeys.ApplicationDirectory)
                : Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.CommonApplicationData),
                        "Sherlock",
                        Path.GetFileNameWithoutExtension(m_ApplicationName));

            m_ApplicationVersion = GetApplicationVersion();
            m_ManifestUri = new Uri(configuration.Value<string>(ServiceConfigurationKeys.UpdateManifestUri));
            m_PublicKeyXml = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                "Sherlock.Service.Properties.ManifestSigningPublicKey.xml");
            m_Updater = m_Container.Resolve<Updater>();

            m_Timer.AutoReset = true;
            m_Timer.Elapsed += TimerElapsed;
            m_Timer.Interval = cycleTimeInMilliSeconds;
            m_Timer.Start();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service stops running.
        /// </summary>
        public void OnStop()
        {
            bool hasBeenStopped;
            lock (m_Lock)
            {
                hasBeenStopped = m_HasBeenStopped;
                m_IsStopping = true;
            }

            if (hasBeenStopped)
            {
                return;
            }

            try
            {
                StopTimer();
                StopApplication();

                m_Diagnostics.Log(
                    LevelToLog.Info,
                    ServiceConstants.LogPrefix,
                    Resources.Log_Messages_ServiceStopped);

                if (m_Container != null)
                {
                    m_Container.Dispose();
                    m_Container = null;
                }
            }
            finally
            {
                lock (m_Lock)
                {
                    m_HasBeenStopped = true;
                }
            }
        }

        private void StopApplication()
        {
            try
            {
                var task = NotifyApplicationOfShutdownAsync();
                task.Wait();
            }
            catch (AggregateException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    ServiceConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_ApplicationShutdownFailed_WithException,
                        e));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                OnStop();

                m_IsDisposed = true;
            }
        }
    }
}
