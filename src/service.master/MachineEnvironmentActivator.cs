//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the base class for classes that activate machine environments, either physical or virtual.
    /// </summary>
    internal abstract class MachineEnvironmentActivator : IEnvironmentActivator
    {
        /// <summary>
        /// The object that stores the application configuration.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The object that stores all the command sets that were retrieved from remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Commands;

        /// <summary>
        /// The object that provides notifications from remote endpoints.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_Notifications;

        /// <summary>
        /// The delegate used to notify the communication system of the disconnection of an endpoint.
        /// </summary>
        private readonly ManualEndpointDisconnection m_Disconnection;

        /// <summary>
        /// The object that stores upload references.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineEnvironmentActivator"/> class.
        /// </summary>
        /// <param name="configuration">The object that stores all the configuration values for the application.</param>
        /// <param name="commands">The object that stores all the command sets that were received from remote endpoints.</param>
        /// <param name="notifications">The object that provides notifications from remote endpoints.</param>
        /// <param name="disconnection">The delegate used to notify the communication system of the disconnection of an endpoint.</param>
        /// <param name="uploads">The object that tracks the files available for upload.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="disconnection"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected MachineEnvironmentActivator(
            IConfiguration configuration,
            ISendCommandsToRemoteEndpoints commands,
            INotifyOfRemoteEndpointEvents notifications,
            ManualEndpointDisconnection disconnection,
            IStoreUploads uploads, 
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.Argument(() => disconnection);
                Lokad.Enforce.Argument(() => uploads);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Configuration = configuration;
            m_Commands = commands;
            m_Notifications = notifications;
            m_Disconnection = disconnection;
            m_Uploads = uploads;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets the object that provides the diagnostics methods for the application.
        /// </summary>
        protected SystemDiagnostics Diagnostics
        {
            get
            {
                return m_Diagnostics;
            }
        }

        /// <summary>
        /// Gets the type of the environment specification that can be handled by the current activator.
        /// </summary>
        public abstract Type EnvironmentTypeToLoad
        {
            get;
        }

        /// <summary>
        /// Creates a new active environment based on the given specification.
        /// </summary>
        /// <param name="environment">The specification that provides the configuration for the active environment.</param>
        /// <param name="sectionBuilder">
        /// The object used to write information to the report about the starting and stopping of the environment.
        /// </param>
        /// <param name="onUnload">The action that is executed upon test completion.</param>
        /// <returns>A new active environment.</returns>
        public IActiveEnvironment Load(
            MachineDescription environment, 
            ITestSectionBuilder sectionBuilder, 
            Action<string> onUnload)
        {
            {
                Lokad.Enforce.Argument(() => environment);
                Lokad.Enforce.Argument(() => onUnload);
            }

            Diagnostics.Log(
                LevelToLog.Trace,
                MachineConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Starting environment: {0}",
                    environment.Name));

            var pingTimeout = m_Configuration.HasValueFor(SharedConfigurationKeys.PingTimeoutInMilliseconds)
                ? m_Configuration.Value<int>(SharedConfigurationKeys.PingTimeoutInMilliseconds)
                : GlobalConstants.DefaultPingTimeoutInMilliseconds;

            var signInTimeout = m_Configuration.HasValueFor(SharedConfigurationKeys.MaximumNetworkSignInTimeInMilliseconds)
                ? m_Configuration.Value<int>(SharedConfigurationKeys.MaximumNetworkSignInTimeInMilliseconds)
                : GlobalConstants.DefaultMaximumNetworkSignInTimeInMilliseconds;

            var cycleTime = m_Configuration.HasValueFor(SharedConfigurationKeys.PingCycleTimeInMilliseconds)
                ? m_Configuration.Value<int>(SharedConfigurationKeys.PingCycleTimeInMilliseconds)
                : GlobalConstants.DefaultPingCycleTimeInMilliseconds;
            var emergencyShutdown = LoadEnvironment(environment, pingTimeout, signInTimeout, cycleTime, sectionBuilder);

            var endpoint = FindMachineEndpoint(environment.NetworkName, signInTimeout, cycleTime);
            if (endpoint == null)
            {
                Diagnostics.Log(
                    LevelToLog.Error,
                    MachineConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Could not find remote endpoint on environment: {0}",
                        environment.Name));

                emergencyShutdown();
                throw new CouldNotLoadEnvironmentException();
            }

            Diagnostics.Log(
                LevelToLog.Info,
                MachineConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Successfully started {0}",
                    environment.Name));

            var commands = m_Commands.CommandsFor<IExecuteTestStepsCommands>(endpoint);
            var notifications = m_Notifications.NotificationsFor<ITestExecutionNotifications>(endpoint);
            return ConstructEnvironmentProxy(
                environment,
                () => DisconnectCommunication(endpoint),
                () => onUnload(environment.Id),
                commands,
                notifications,
                m_Uploads,
                sectionBuilder);
        }

        /// <summary>
        /// Load the environment.
        /// </summary>
        /// <param name="environment">The specification for the environment.</param>
        /// <param name="pingTimeoutInMilliseconds">The number of milliseconds that the ping operation should wait for a response.</param>
        /// <param name="maximumWaitTimeInMilliseconds">
        ///     The number of milliseconds that the operation should wait for the machine to come online.
        /// </param>
        /// <param name="pingCycleTimeInMilliseconds">The amount of time the operation should wait between consecutive ping operations.</param>
        /// <param name="sectionBuilder">
        /// The object used to write information to the report about the starting and stopping of the environment.
        /// </param>
        /// <returns>An action that should be used to return the environment to its previous state in case the environment load fails.</returns>
        protected abstract Action LoadEnvironment(
            MachineDescription environment,
            int pingTimeoutInMilliseconds,
            int maximumWaitTimeInMilliseconds,
            int pingCycleTimeInMilliseconds,
            ITestSectionBuilder sectionBuilder);

        private EndpointId FindMachineEndpoint(string networkName, int totalWaitTimeInMilliseconds, int cycleTimeInMilliseconds)
        {
            // See if we have anything from the machine yet. There are two situations. Either 
            // - The machine was already up and running and communication has been established. In that case the command information
            //   is already available in the collection.
            // - The machine is still starting up (a successful ping response only means the basics are up and running, it doesn't mean
            //   the services are there). In that case we should be getting the information shortly, but we may have to wait a bit.
            EndpointId endpoint = null;
            EventHandler<CommandSetAvailabilityEventArgs> handler = (s, e) =>
            {
                if (e.Endpoint.IsOnMachine(networkName))
                {
                    endpoint = e.Endpoint;
                }
            };
            m_Commands.OnEndpointSignedIn += handler;
            try
            {
                // See if we already have the information
                var knownCommands = m_Commands.AvailableCommands().ToList();
                foreach (var pair in knownCommands)
                {
                    if (pair.Endpoint.IsOnMachine(networkName))
                    {
                        endpoint = pair.Endpoint;
                        break;
                    }
                }

                // If the information isn't there, then we wait for it
                if (endpoint == null)
                {
                    MachineHelpers.WaitForResponse(
                        () => endpoint != null,
                        Diagnostics,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Endpoint on {0} signed in",
                            networkName),
                        totalWaitTimeInMilliseconds,
                        cycleTimeInMilliseconds);
                }
            }
            finally
            {
                m_Commands.OnEndpointSignedIn -= handler;
            }

            return endpoint;
        }

        private void DisconnectCommunication(EndpointId endpoint)
        {
            var resetEvent = new AutoResetEvent(false);
            EventHandler<EndpointEventArgs> handler = (s, e) =>
            {
                if (e.Endpoint.Equals(endpoint))
                {
                    resetEvent.Set();
                }
            };
            m_Commands.OnEndpointSignedOff += handler;
            try
            {
                // See if we already have the information
                if (m_Commands.HasCommandsFor(endpoint))
                {
                    m_Disconnection(endpoint, ChannelType.TcpIP);
                    resetEvent.WaitOne(GlobalConstants.DefaultMaximumNetworkSignInTimeInMilliseconds);
                }
            }
            finally
            {
                m_Commands.OnEndpointSignedOff -= handler;
            }
        }

        /// <summary>
        /// Constructs a new active environment proxy.
        /// </summary>
        /// <param name="environment">The specification for the environment.</param>
        /// <param name="preTerminateEnvironment">The action executed just prior to terminating the environment.</param>
        /// <param name="postTerminateEnvironment">The action executed after terminating the environment.</param>
        /// <param name="commands">The object that provides the commands used to communicate with the environment.</param>
        /// <param name="notifications">The object that provides notifications from the environment.</param>
        /// <param name="uploads">The object that tracks the files available for upload.</param>
        /// <param name="sectionBuilder">
        /// The object used to write information to the report about the starting and stopping of the environment.
        /// </param>
        /// <returns>A new active environment proxy object.</returns>
        protected abstract IActiveEnvironment ConstructEnvironmentProxy(
            MachineDescription environment,
            Action preTerminateEnvironment,
            Action postTerminateEnvironment,
            IExecuteTestStepsCommands commands,
            ITestExecutionNotifications notifications,
            IStoreUploads uploads,
            ITestSectionBuilder sectionBuilder);
    }
}
