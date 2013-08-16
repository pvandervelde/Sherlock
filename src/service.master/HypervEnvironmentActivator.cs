//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
    /// An <see cref="IEnvironmentActivator"/> that is able to activate Hyper-V virtual machine environments.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hyperv",
        Justification = "Hyper-V isn't legal either. This is the least silly version.")]
    internal sealed class HypervEnvironmentActivator : MachineEnvironmentActivator
    {
        /// <summary>
        /// The function that retrieves environment specifications based on their ID.
        /// </summary>
        private readonly Func<string, MachineDescription> m_EnvironmentById;

        /// <summary>
        /// Initializes a new instance of the <see cref="HypervEnvironmentActivator"/> class.
        /// </summary>
        /// <param name="configuration">The object that stores all the configuration values for the application.</param>
        /// <param name="commands">The object that stores all the command sets that were received from remote endpoints.</param>
        /// <param name="notifications">The object that provides notifications from remote endpoints.</param>
        /// <param name="disconnection">The delegate used to notify the communication system of the disconnection of an endpoint.</param>
        /// <param name="uploads">The object that tracks the files available for upload.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="environmentById">The function that is used to find environments based on their ID.</param>
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
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="environmentById"/> is <see langword="null" />.
        /// </exception>
        public HypervEnvironmentActivator(
            IConfiguration configuration,
            ISendCommandsToRemoteEndpoints commands,
            INotifyOfRemoteEndpointEvents notifications,
            ManualEndpointDisconnection disconnection,
            IStoreUploads uploads,
            SystemDiagnostics diagnostics,
            Func<string, MachineDescription> environmentById)
            : base(configuration, commands, notifications, disconnection, uploads, diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => environmentById);
            }

            m_EnvironmentById = environmentById;
        }

        /// <summary>
        /// Gets the type of the environment specification that can be handled by the current activator.
        /// </summary>
        public override Type EnvironmentTypeToLoad
        {
            get
            {
                return typeof(HypervMachineDescription);
            }
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
        protected override Action LoadEnvironment(
            MachineDescription environment,
            int pingTimeoutInMilliseconds,
            int maximumWaitTimeInMilliseconds,
            int pingCycleTimeInMilliseconds,
            ITestSectionBuilder sectionBuilder)
        {
            var specification = environment as HypervMachineDescription;
            if (specification == null)
            {
                throw new InvalidEnvironmentSpecificationException();
            }

            // See if the host machine is alive
            var host = specification.HostMachineId;
            var hostSpecification = m_EnvironmentById(host);
            if (hostSpecification == null)
            {
                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to locate host machine: {0}",
                        hostSpecification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                throw new CouldNotLoadEnvironmentException();
            }

            // See if the host is awake and if not try to wake it up.
            var physicalHostSpecification = hostSpecification as PhysicalMachineDescription;
            var canStartRemotely = physicalHostSpecification != null && physicalHostSpecification.CanStartRemotely;
            if (!MachineHelpers.WakeNetworkedMachineAndWaitForNetworkSignIn(
                hostSpecification.NetworkName,
                hostSpecification.MacAddress,
                Diagnostics,
                canStartRemotely,
                pingTimeoutInMilliseconds,
                maximumWaitTimeInMilliseconds,
                pingCycleTimeInMilliseconds))
            {
                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to start host machine: {0}",
                        hostSpecification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                throw new CouldNotLoadEnvironmentException();
            }

            // Connect to the Hyper-V host for the given environment
            var virtualMachine = new HypervVirtualMachine(specification.Image);
            if (virtualMachine.State == HypervVirtualMachineState.Paused || virtualMachine.State == HypervVirtualMachineState.Running)
            {
                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Virtual machine already in use: {0}",
                        specification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                // Throw an error, because we don't know what state the snapshots are in, we can't reset the machine
                // so the only way out is to error
                throw new EnvironmentAlreadyInUseException();
            }

            virtualMachine.Start();

            Diagnostics.Log(
                LevelToLog.Debug,
                HyperVConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Started Hyper-V virtual machine {0} [image: {1}] on host {2}",
                    specification.Name,
                    specification.Image,
                    hostSpecification.Name));

            sectionBuilder.AddInformationMessage(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Started machine: {0}",
                    specification.NetworkName));

            return () =>
            {
                Diagnostics.Log(
                    LevelToLog.Debug,
                    HyperVConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Terminating Hyper-V virtual machine {0} [image: {1}] on host {2}",
                        specification.Name,
                        specification.Image,
                        hostSpecification.Name));

                virtualMachine.Terminate();

                DateTimeOffset killTime = DateTimeOffset.Now + TimeSpan.FromMilliseconds(GlobalConstants.DefaultMaximumMachineShutdownTime);
                while ((virtualMachine.State != HypervVirtualMachineState.TurnedOff) && (DateTimeOffset.Now <= killTime))
                {
                    Thread.Sleep(10);
                }

                virtualMachine.RestoreToSnapshot(specification.SnapshotToReturnTo);

                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Terminated machine: {0}",
                        specification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);
            };
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
        protected override IActiveEnvironment ConstructEnvironmentProxy(
            MachineDescription environment,
            Action preTerminateEnvironment,
            Action postTerminateEnvironment,
            IExecuteTestStepsCommands commands,
            ITestExecutionNotifications notifications,
            IStoreUploads uploads,
            ITestSectionBuilder sectionBuilder)
        {
            Action shutDownAction =
                () =>
                {
                    preTerminateEnvironment();
                    ShutdownVirtualMachine(environment, sectionBuilder);
                    postTerminateEnvironment();
                };

            return new ActiveHypervEnvironment(
                environment.Id,
                shutDownAction,
                commands,
                notifications,
                uploads);
        }

        private void ShutdownVirtualMachine(MachineDescription environment, ITestSectionBuilder sectionBuilder)
        {
            var specification = environment as HypervMachineDescription;
            if (specification == null)
            {
                throw new InvalidEnvironmentSpecificationException();
            }

            // See if the host machine is alive
            var host = specification.HostMachineId;
            var hostSpecification = m_EnvironmentById(host);
            if (hostSpecification == null)
            {
                sectionBuilder.AddErrorMessage(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not locate VM host: {0}",
                                hostSpecification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                throw new CouldNotLoadEnvironmentException();
            }

            Diagnostics.Log(
                LevelToLog.Debug,
                HyperVConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Terminating Hyper-V virtual machine {0} [image: {1}] on host {2}",
                    specification.Name,
                    specification.Image,
                    hostSpecification.Name));

            // Connect to the Hyper-V host for the given environment
            var virtualMachine = new HypervVirtualMachine(specification.Image);
            try
            {
                virtualMachine.Terminate();
            }
            catch (FailedToRestoreEnvironmentException)
            {
                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to shut down virtual machine: {0}",
                        specification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                throw;
            }

            var killTime = DateTimeOffset.Now + TimeSpan.FromMilliseconds(GlobalConstants.DefaultMaximumMachineShutdownTime);
            while ((virtualMachine.State != HypervVirtualMachineState.TurnedOff) && (DateTimeOffset.Now <= killTime))
            {
                Thread.Sleep(10);
            }

            try
            {
                virtualMachine.RestoreToSnapshot(specification.SnapshotToReturnTo);
            }
            catch (FailedToRestoreEnvironmentException)
            {
                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to restore virtual machine snapshot [{0}] for machine {1}",
                        specification.SnapshotToReturnTo,
                        specification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                throw;
            }

            Diagnostics.Log(
                LevelToLog.Debug,
                HyperVConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Hyper-V virtual machine {0} terminated",
                    specification.Name));

            sectionBuilder.AddInformationMessage(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Virtual machine shut down: {0}",
                    specification.NetworkName));
            sectionBuilder.FinalizeAndStore(true);
        }
    }
}
