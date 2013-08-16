//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Management;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Stores information about a Hyper-V virtual machine.
    /// </summary>
    internal sealed class HypervVirtualMachine
    {
        private static ManagementObject GetWmiObject(string virtualMachineId)
        {
            var scope = new ManagementScope(@"root\virtualization", null);
            var vmObject = WmiUtility.GetTargetComputer(virtualMachineId, scope);
            if (vmObject == null)
            {
                throw new FailedToLocateEnvironmentException();
            }

            return vmObject;
        }

        /// <summary>
        /// The ID of the virtual machine.
        /// </summary>
        private readonly string m_VirtualMachineId;

        /// <summary>
        /// Initializes a new instance of the <see cref="HypervVirtualMachine"/> class.
        /// </summary>
        /// <param name="virtualMachineId">The ID of the virtual machine.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="virtualMachineId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="virtualMachineId"/> is an empty string.
        /// </exception>
        public HypervVirtualMachine(string virtualMachineId)
        {
            {
                Lokad.Enforce.Argument(() => virtualMachineId);
                Lokad.Enforce.Argument(() => virtualMachineId, Lokad.Rules.StringIs.NotEmpty);
            }

            m_VirtualMachineId = virtualMachineId;
        }

        /// <summary>
        /// Gets the state of the virtual machine.
        /// </summary>
        public HypervVirtualMachineState State
        {
            get
            {
                var result = (ushort)GetWmiObject(m_VirtualMachineId)["EnabledState"];
                return (HypervVirtualMachineState)result;
            }
        }

        /// <summary>
        /// Starts the virtual machine.
        /// </summary>
        public void Start()
        {
            try
            {
                ChangeMachineState(HypervVirtualMachineStateChange.Start);
            }
            catch (ManagementException)
            {
                throw new CouldNotLoadEnvironmentException();
            }
        }

        private void ChangeMachineState(HypervVirtualMachineStateChange newState)
        {
            var virtualMachine = GetWmiObject(m_VirtualMachineId);
            var inputParameters = virtualMachine.GetMethodParameters("RequestStateChange");
            inputParameters["RequestedState"] = (ushort)newState;
            virtualMachine.InvokeMethod("RequestStateChange", inputParameters, null);
        }

        /// <summary>
        /// Terminates the virtual machine.
        /// </summary>
        public void Terminate()
        {
            try
            {
                ChangeMachineState(HypervVirtualMachineStateChange.Terminate);
            }
            catch (ManagementException)
            {
                throw new FailedToRestoreEnvironmentException();
            }
        }

        /// <summary>
        /// Restores the state of the virtual machine to the given snapshot.
        /// </summary>
        /// <param name="snapshot">The name of the snapshot that should be restored.</param>
        public void RestoreToSnapshot(string snapshot)
        {
            var virtualMachine = GetWmiObject(m_VirtualMachineId);
            var virtualSystemsetting = WmiUtility.GetSnapshotData(virtualMachine, snapshot);
            if (virtualSystemsetting == null)
            {
                throw new FailedToRestoreEnvironmentException(
                    Resources.Exceptions_Messages_HypervVirtualMachine_CouldNotGetSnapshot);
            }

            var scope = new ManagementScope(@"root\virtualization", null);
            var virtualSystemService = WmiUtility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");

            var inParams = virtualSystemService.GetMethodParameters("ApplyVirtualSystemSnapshot");
            inParams["SnapshotSettingData"] = virtualSystemsetting.Path.Path;
            inParams["ComputerSystem"] = virtualMachine.Path.Path;

            var outParams = virtualSystemService.InvokeMethod("ApplyVirtualSystemSnapshot", inParams, null);
            var returnCode = (WmiReturnCode)(uint)outParams["ReturnValue"];
            switch (returnCode)
            {
                case WmiReturnCode.Started:
                    if (!WmiUtility.JobCompleted(outParams, scope))
                    {
                        throw new FailedToRestoreEnvironmentException(
                            Resources.Exceptions_Messages_HypervVirtualMachine_JobFailedToComplete);
                    }

                    break;
                case WmiReturnCode.Completed:
                    break;
                default:
                    throw new FailedToRestoreEnvironmentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_HypervVirtualMachine_UnknownReturnCode,
                            returnCode));
            }
        }
    }
}
