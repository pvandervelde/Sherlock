//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// An <see cref="IEnvironmentActivator"/> that is able to activate physical machine environments.
    /// </summary>
    internal sealed class PhysicalMachineEnvironmentActivator : MachineEnvironmentActivator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalMachineEnvironmentActivator"/> class.
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
        public PhysicalMachineEnvironmentActivator(
            IConfiguration configuration,
            ISendCommandsToRemoteEndpoints commands,
            INotifyOfRemoteEndpointEvents notifications,
            ManualEndpointDisconnection disconnection,
            IStoreUploads uploads,
            SystemDiagnostics diagnostics) 
            : base(configuration, commands, notifications, disconnection, uploads, diagnostics)
        {
        }

        /// <summary>
        /// Gets the type of the environment specification that can be handled by the current activator.
        /// </summary>
        public override Type EnvironmentTypeToLoad
        {
            get
            {
                return typeof(PhysicalMachineDescription);
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
            var specification = environment as PhysicalMachineDescription;
            if (specification == null)
            {
                throw new InvalidEnvironmentSpecificationException();
            }

            if (!MachineHelpers.WakeNetworkedMachineAndWaitForNetworkSignIn(
                specification.NetworkName,
                specification.MacAddress,
                Diagnostics,
                specification.CanStartRemotely,
                pingTimeoutInMilliseconds,
                maximumWaitTimeInMilliseconds,
                pingCycleTimeInMilliseconds))
            {
                sectionBuilder.AddErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to start machine: {0}",
                        specification.NetworkName));
                sectionBuilder.FinalizeAndStore(false);

                throw new CouldNotLoadEnvironmentException();
            }

            sectionBuilder.AddInformationMessage(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Started machine: {0}",
                    specification.NetworkName));

            return () =>
            {
                // Do nothing really. We can't shut machines down remotely.
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
                    postTerminateEnvironment();

                    sectionBuilder.AddInformationMessage(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Terminated machine: {0}",
                            environment.NetworkName));
                    sectionBuilder.FinalizeAndStore(true);
                };

            return new ActivePhysicalMachineEnvironment(
                environment.Id,
                shutDownAction,
                commands,
                notifications,
                uploads);
        }
    }
}
