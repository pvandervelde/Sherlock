//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines utility methods for WMI.
    /// </summary>
    internal static class WmiUtility
    {
        /// <summary>
        /// Common utility function to get a service object.
        /// </summary>
        /// <param name="scope">The management scope that is used to connect to WMI.</param>
        /// <param name="serviceName">The name of the service through which the object should be obtained.</param>
        /// <returns>The desired management object, or <see langword="null" />.</returns>
        public static ManagementObject GetServiceObject(ManagementScope scope, string serviceName)
        {
            scope.Connect();
            var wmiPath = new ManagementPath(serviceName);
            var serviceClass = new ManagementClass(scope, wmiPath, null);
            var services = serviceClass.GetInstances();

            ManagementObject serviceObject = null;
            foreach (ManagementObject service in services)
            {
                serviceObject = service;
            }

            return serviceObject;
        }

        /// <summary>
        /// Determines if the WMI job has been completed.
        /// </summary>
        /// <param name="outParams">The output parameters which store the job information.</param>
        /// <param name="scope">The management scope.</param>
        /// <returns>
        /// <see langword="true" /> if the job has been completed; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool JobCompleted(ManagementBaseObject outParams, ManagementScope scope)
        {
            bool jobCompleted = true;

            // Retrieve msvc_StorageJob path. This is a full wmi path
            var jobPath = (string)outParams["Job"];
            var job = new ManagementObject(scope, new ManagementPath(jobPath), null);
            
            // Try to get storage job information
            job.Get();
            while ((WmiJobState)job["JobState"] == WmiJobState.Starting || (WmiJobState)job["JobState"] == WmiJobState.Running)
            {
                System.Threading.Thread.Sleep(1000);
                job.Get();
            }

            // Figure out if job failed
            var jobState = (WmiJobState)job["JobState"];
            if (jobState != WmiJobState.Completed)
            {
                jobCompleted = false;
            }
            
            return jobCompleted;
        }

        /// <summary>
        /// Gets the target virtual machine.
        /// </summary>
        /// <param name="vmElementName">The name of the virtual machine.</param>
        /// <param name="scope">The management scope.</param>
        /// <returns>The virtual machine object, or <see langword="null" /> if no machine could be found.</returns>
        public static ManagementObject GetTargetComputer(string vmElementName, ManagementScope scope)
        {
            var query = string.Format(
                CultureInfo.InvariantCulture,
                "select * from Msvm_ComputerSystem Where ElementName = '{0}'", 
                vmElementName);
            var searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query));
            var computers = searcher.Get();

            ManagementObject computer = null;
            foreach (ManagementObject instance in computers)
            {
                computer = instance;
                break;
            }

            return computer;
        }

        /// <summary>
        /// Gets the data for a given snapshot that belongs to the given virtual machine..
        /// </summary>
        /// <param name="virtualMachine">The virtual machine object.</param>
        /// <param name="snapshotName">The name of the snapshot that should be obtained.</param>
        /// <returns>The snapshot data object or <see langword="null" /> if no snapshot could be found.</returns>
        public static ManagementObject GetSnapshotData(ManagementObject virtualMachine, string snapshotName)
        {
            ManagementObjectCollection vmSettings = virtualMachine.GetRelated(
                "Msvm_VirtualSystemsettingData",
                "Msvm_PreviousSettingData",
                null,
                null,
                "SettingData",
                "ManagedElement",
                false,
                null);

            ManagementObject vmSetting = null;
            foreach (ManagementObject instance in vmSettings)
            {
                var name = (string)instance["ElementName"];
                if (string.Equals(snapshotName, name, StringComparison.Ordinal))
                {
                    vmSetting = instance;
                    break;
                }
            }

            return vmSetting;
        }
    }
}
