//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public Environment API for the Sherlock Context class.
    /// </content>
    internal sealed partial class SherlockContext : IProvideEnvironmentContext
    {
        private static void CopyPropertyValues(OperatingSystemDescription stored, OperatingSystemDescription operatingSystem)
        {
            stored.Name = operatingSystem.Name;
            stored.ServicePack = operatingSystem.ServicePack;
            stored.PointerSize = operatingSystem.PointerSize;
            stored.CultureInfo = operatingSystem.CultureInfo;
        }

        /// <summary>
        /// Updates the stored machine with the data from the changed machine, and indicates if the
        /// stored machine should be patched.
        /// </summary>
        /// <param name="stored">The machine object that was stored in the database.</param>
        /// <param name="changed">The machine object that was changed.</param>
        /// <returns>
        /// A value indicating if the stored value should be patched or not.
        /// </returns>
        private static bool Update(MachineDescription stored, MachineDescription changed)
        {
            stored.Name = changed.Name;
            stored.Description = changed.Description;
            stored.NetworkName = changed.NetworkName;
            stored.MacAddress = changed.MacAddress;
            stored.IsAvailableForTesting = changed.IsAvailableForTesting;

            if (stored.OperatingSystemId != changed.OperatingSystemId)
            {
                stored.OperatingSystem.Machines.Remove(stored);

                stored.OperatingSystemId = changed.OperatingSystemId;
                stored.OperatingSystem = null;

                return true;
            }

            return false;
        }

        private void InitializeEnvironments()
        {
            var applications = GetApplicationsById(null).ToList();
            foreach (var application in applications)
            {
                Patch(application);
            }

            var operatingSystems = GetOperatingSystemsById(null).ToList();
            foreach (var operatingSystem in operatingSystems)
            {
                Patch(operatingSystem);
            }

            var physicalMachines = GetPhysicalMachines(null).ToList();
            foreach (var physicalMachine in physicalMachines)
            {
                Patch(physicalMachine);
            }

            var hypervMachines = GetHypervMachines(null).ToList();
            foreach (var hypervMachine in hypervMachines)
            {
                Patch(hypervMachine);
            }
        }

        private HypervMachineDescription Patch(HypervMachineDescription description)
        {
            var result = (StoredMachineDescriptions.Find(description.Id) ?? StoredMachineDescriptions.Add(description)) as HypervMachineDescription;
            Debug.Assert(result != null, "The machine should be a Hyper-V machine.");

            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    result.HypervMachines = new List<HypervMachineDescription>();

                    var selectedMachineApplications = GetMachineApplicationByMachineId(result.Id)
                        .Select(Patch)
                        .ToList();
                    result.MachineApplications = selectedMachineApplications;

                    var selectedOperatingSystem = GetOperatingSystemsById(result.OperatingSystemId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.OperatingSystem = selectedOperatingSystem;
                    result.HostMachine = Machine(result.HostMachineId);

                    var selectedTestEnvironments = GetTestEnvironmentByMachineId(result.Id)
                        .Select(Patch)
                        .ToList();
                    result.TestEnvironments = selectedTestEnvironments;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private PhysicalMachineDescription Patch(PhysicalMachineDescription description)
        {
            var result = (StoredMachineDescriptions.Find(description.Id) ?? StoredMachineDescriptions.Add(description)) as PhysicalMachineDescription;
            Debug.Assert(result != null, "The machine should be a physical machine.");

            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedHypervMachines = GetHypervMachinesByHostId(result.Id)
                        .Select(Patch)
                        .ToList();
                    result.HypervMachines = selectedHypervMachines;

                    var selectedMachineApplications = GetMachineApplicationByMachineId(result.Id)
                        .Select(Patch)
                        .ToList();
                    result.MachineApplications = selectedMachineApplications;

                    var selectedOperatingSystem = GetOperatingSystemsById(result.OperatingSystemId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.OperatingSystem = selectedOperatingSystem;

                    var selectedTestEnvironments = GetTestEnvironmentByMachineId(result.Id)
                        .Select(Patch)
                        .ToList();
                    result.TestEnvironments = selectedTestEnvironments;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private OperatingSystemDescription Patch(OperatingSystemDescription description)
        {
            var result = StoredOperatingSystemDescriptions.Find(description.Id) ?? StoredOperatingSystemDescriptions.Add(description);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedMachines = GetMachinesByOperatingSystemId(result.Id)
                        .Select(Machine)
                        .ToList();
                    result.Machines = selectedMachines;

                    var selectedEnvironments = GetTestEnvironmentByOperatingSystemId(result.Id)
                        .Select(id => TestEnvironment(id.Value))
                        .ToList();
                    result.TestEnvironments = selectedEnvironments;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return description;
        }

        private MachineApplication Patch(MachineApplication description)
        {
            var result = StoredMachineApplications.Find(description.Id) ?? StoredMachineApplications.Add(description);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    if ((result.Application == null) || (result.Application.Id != result.ApplicationId))
                    {
                        var selectedApplication = Application(result.ApplicationId);
                        result.Application = selectedApplication;
                    }

                    if ((result.Machine == null) || (result.Machine.Id != result.MachineId))
                    {
                        var selectedMachine = Machine(result.MachineId);
                        result.Machine = selectedMachine;
                    }

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private ApplicationDescription Patch(ApplicationDescription description)
        {
            var result = StoredApplicationDescriptions.Find(description.Id) ?? StoredApplicationDescriptions.Add(description);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedMachineApplications = GetMachineApplicationByApplicationId(result.Id)
                        .Select(Patch)
                        .ToList();
                    result.MachineApplications = selectedMachineApplications;

                    var selectedTestApplications = GetTestApplicationsByApplicationId(result.Id)
                        .Select(id => TestApplication(id.Value))
                        .ToList();
                    result.TestApplications = selectedTestApplications;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds a new physical machine.
        /// </summary>
        /// <param name="machine">The new machine.</param>
        public void Add(PhysicalMachineDescription machine)
        {
            VerifySchemaVersion();

            var result = StoredMachineDescriptions.Add(machine) as PhysicalMachineDescription;
            Debug.Assert(result != null, "The machine should be a physical machine.");

            Patch(result);
        }

        /// <summary>
        /// Adds a new Hyper-V machine.
        /// </summary>
        /// <param name="machine">The new machine.</param>
        public void Add(HypervMachineDescription machine)
        {
            VerifySchemaVersion();

            var result = StoredMachineDescriptions.Add(machine) as HypervMachineDescription;
            Debug.Assert(result != null, "The machine should be a Hyper-V machine.");

            Patch(result);
        }

        /// <summary>
        /// Returns the machine with the given ID.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The desired machine.</returns>
        public MachineDescription Machine(string id)
        {
            VerifySchemaVersion();

            var machine = StoredMachineDescriptions.Find(id);
            if (machine != null)
            {
                return machine;
            }

            var physicalMachine = GetPhysicalMachines(id).FirstOrDefault();
            if (physicalMachine != null)
            {
                return Patch(physicalMachine);
            }

            var hypervMachine = GetHypervMachines(id).FirstOrDefault();
            if (hypervMachine != null)
            {
                return Patch(hypervMachine);
            }

            return null;
        }

        /// <summary>
        /// Returns a collection containing all the available machines.
        /// </summary>
        /// <returns>A collection containing all the available machines.</returns>
        public IEnumerable<MachineDescription> Machines()
        {
            VerifySchemaVersion();

            return StoredMachineDescriptions
                .ToList();
        }

        /// <summary>
        /// Returns a collection containing all the available physical machines.
        /// </summary>
        /// <returns>A collection containing all the available physical machines.</returns>
        public IEnumerable<MachineDescription> PhysicalMachines()
        {
            VerifySchemaVersion();

            return StoredMachineDescriptions
                .Where(m => m is PhysicalMachineDescription)
                .ToList();
        }

        /// <summary>
        /// Returns a collection containing all the available Hyper-V machines.
        /// </summary>
        /// <returns>A collection containing all the available Hyper-V machines.</returns>
        public IEnumerable<MachineDescription> HypervMachines()
        {
            VerifySchemaVersion();

            return StoredMachineDescriptions
                .Where(m => m is HypervMachineDescription)
                .ToList();
        }

        /// <summary>
        /// Returns a collection containing all the inactive machines that have the specified operating system
        /// and the specified set of applications.
        /// </summary>
        /// <param name="operatingSystem">The required operation system.</param>
        /// <param name="applications">The collection of required applications.</param>
        /// <returns>A collection containing all inactive machines with the specified operating system and the specified applications.</returns>
        public IEnumerable<MachineDescription> InactiveMachinesWith(
            OperatingSystemDescription operatingSystem,
            IEnumerable<ApplicationDescription> applications)
        {
            VerifySchemaVersion();

            // Make sure we have a complete OperatingSystemDescription object, including IDs
            var storedOs = GetStoredOperatingSystemFromData(operatingSystem);

            // Make sure we have complete ApplicationDescription objects, including IDs
            var storedApplications = new List<ApplicationDescription>();
            foreach (var app in applications)
            {
                var storedApp = GetStoredApplicationFromData(app);
                storedApplications.Add(storedApp);
            }

            var selectedMachines = (IEnumerable<string>)GetMachinesByOperatingSystemId(storedOs.Id);
            foreach (var app in storedApplications)
            {
                var machinesWithApp = GetMachinesByApplicationId(app.Id);
                selectedMachines = selectedMachines.Intersect(machinesWithApp);
                if (!selectedMachines.Any())
                {
                    throw new FailedToLocateEnvironmentException();
                }
            }

            return selectedMachines
                .Select(Machine)
                .ToList();
        }

        private OperatingSystemDescription GetStoredOperatingSystemFromData(OperatingSystemDescription operatingSystem)
        {
            var storedOs = GetOperatingSystemsByName(
                operatingSystem.Name,
                operatingSystem.ServicePack,
                operatingSystem.ArchitecturePointerSize,
                operatingSystem.Culture)
                .FirstOrDefault();
            if (storedOs == null)
            {
                throw new UnknownOperatingSystemException();
            }

            return Patch(storedOs);
        }

        private ApplicationDescription GetStoredApplicationFromData(ApplicationDescription app)
        {
            var storedApp = GetApplicationsByName(
                app.Name,
                app.Version.Major,
                app.Version.Minor,
                app.Version.Build,
                app.Version.Revision)
                .FirstOrDefault();
            if (storedApp == null)
            {
                throw new UnknownApplicationException();
            }

            return Patch(storedApp);
        }

        /// <summary>
        /// Marks the given machine as active.
        /// </summary>
        /// <param name="machine">The ID of the machine.</param>
        public void MarkMachineAsActive(string machine)
        {
            VerifySchemaVersion();

            SwitchMachineToActive(machine);
        }

        /// <summary>
        /// Marks the given machine as inactive.
        /// </summary>
        /// <param name="machine">The ID of the machine.</param>
        public void MarkMachineAsInactive(string machine)
        {
            VerifySchemaVersion();

            SwitchMachineToInactive(machine);
        }

        /// <summary>
        /// Updates the machine with the data from the given object.
        /// </summary>
        /// <param name="machine">The machine.</param>
        public void Update(MachineDescription machine)
        {
            VerifySchemaVersion();

            var storedMachine = Machine(machine.Id);
            if (storedMachine != null)
            {
                if (!ReferenceEquals(storedMachine, machine))
                {
                    var storedPhysicalMachine = storedMachine as PhysicalMachineDescription;
                    if (storedPhysicalMachine != null)
                    {
                        Update(storedPhysicalMachine, machine as PhysicalMachineDescription);
                    }

                    var storedHypervMachine = storedMachine as HypervMachineDescription;
                    if (storedHypervMachine != null)
                    {
                        Update(storedHypervMachine, machine as HypervMachineDescription);
                    }
                }

                var entry = Entry(storedMachine);
                entry.State = EntityState.Modified;
            }
        }

        private void Update(PhysicalMachineDescription stored, PhysicalMachineDescription changed)
        {
            var shouldPatch = Update(stored as MachineDescription, changed as MachineDescription);
            stored.CanStartRemotely = changed.CanStartRemotely;

            if (shouldPatch)
            {
                stored.IsPatched = false;
                Patch(stored);
            }
        }

        private void Update(HypervMachineDescription stored, HypervMachineDescription changed)
        {
            var shouldPatch = Update(stored as MachineDescription, changed as MachineDescription);
            stored.Image = changed.Image;
            stored.SnapshotToReturnTo = changed.SnapshotToReturnTo;
            if (!string.Equals(stored.HostMachineId, changed.HostMachineId, StringComparison.Ordinal))
            {
                stored.HostMachine.HypervMachines.Remove(stored);

                stored.HostMachineId = changed.HostMachineId;
                stored.HostMachine = null;

                shouldPatch = true;
            }

            if (shouldPatch)
            {
                stored.IsPatched = false;
                Patch(stored);
            }
        }

        /// <summary>
        /// Deletes an existing machine.
        /// </summary>
        /// <remarks>
        /// Deleting an machine may result in cascading deletes, e.g. when deleting a physical machine
        /// which is a host for one or more Hyper-V machines.
        /// </remarks>
        /// <param name="machine">The ID of the machine.</param>
        public void DeleteMachine(string machine)
        {
            VerifySchemaVersion();

            var description = Machine(machine);
            if (description != null)
            {
                StoredMachineDescriptions.Remove(description);
            }
        }

        /// <summary>
        /// Adds a new operating system.
        /// </summary>
        /// <param name="operatingSystem">The new operating system.</param>
        public void Add(OperatingSystemDescription operatingSystem)
        {
            VerifySchemaVersion();

            var result = StoredOperatingSystemDescriptions.Add(operatingSystem);
            Patch(result);
        }

        /// <summary>
        /// Returns a collection containing all the available operating systems.
        /// </summary>
        /// <returns>A collection containing all the operating systems.</returns>
        public IEnumerable<OperatingSystemDescription> OperatingSystems()
        {
            VerifySchemaVersion();

            return StoredOperatingSystemDescriptions
                .ToList();
        }

        /// <summary>
        /// Returns the operating system with the given ID.
        /// </summary>
        /// <param name="id">The operating system ID.</param>
        /// <returns>The requested operating system.</returns>
        public OperatingSystemDescription OperatingSystem(int id)
        {
            VerifySchemaVersion();

            var description = StoredOperatingSystemDescriptions.Find(id) ?? GetOperatingSystemsById(id).FirstOrDefault();
            return (description != null) ? Patch(description) : null;
        }

        /// <summary>
        /// Updates the operating system with the data from the given object.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        public void Update(OperatingSystemDescription operatingSystem)
        {
            VerifySchemaVersion();

            var storedOperatingSystem = OperatingSystem(operatingSystem.Id);
            if (storedOperatingSystem != null)
            {
                if (!ReferenceEquals(storedOperatingSystem, operatingSystem))
                {
                    CopyPropertyValues(storedOperatingSystem, operatingSystem);
                }

                var entry = Entry(storedOperatingSystem);
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes an existing operating system.
        /// </summary>
        /// <remarks>
        /// Deleting an operating system may result in cascading deletes elsewhere, e.g. when deleting an 
        /// operating system that is used by one or more machines.
        /// </remarks>
        /// <param name="id">The ID of the operating system.</param>
        public void DeleteOperatingSystem(int id)
        {
            VerifySchemaVersion();

            var description = OperatingSystem(id);
            if (description != null)
            {
                StoredOperatingSystemDescriptions.Remove(description);
            }
        }

        /// <summary>
        /// Adds a new application.
        /// </summary>
        /// <param name="application">The new application.</param>
        public void Add(ApplicationDescription application)
        {
            VerifySchemaVersion();

            var result = StoredApplicationDescriptions.Add(application);
            Patch(result);
        }

        /// <summary>
        /// Returns a collection containing all the available applications.
        /// </summary>
        /// <returns>A collection containing all the applications.</returns>
        public IEnumerable<ApplicationDescription> Applications()
        {
            VerifySchemaVersion();

            return StoredApplicationDescriptions
                .ToList();
        }

        /// <summary>
        /// Returns the application with the given ID.
        /// </summary>
        /// <param name="id">The application ID.</param>
        /// <returns>The requested application.</returns>
        public ApplicationDescription Application(int id)
        {
            VerifySchemaVersion();

            var description = StoredApplicationDescriptions.Find(id) ?? GetApplicationsById(id).FirstOrDefault();
            return (description != null) ? Patch(description) : null;
        }

        /// <summary>
        /// Updates the application with the data from the given object.
        /// </summary>
        /// <param name="application">The application.</param>
        public void Update(ApplicationDescription application)
        {
            VerifySchemaVersion();

            var storedApplication = Application(application.Id);
            if (storedApplication != null)
            {
                if (!ReferenceEquals(storedApplication, application))
                {
                    storedApplication.Name = application.Name;
                    storedApplication.Version = application.Version;
                }

                var entry = Entry(storedApplication);
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        public void DeleteApplication(int id)
        {
            VerifySchemaVersion();

            var description = Application(id);
            if (description != null)
            {
                StoredApplicationDescriptions.Remove(description);
            }
        }
    }
}
