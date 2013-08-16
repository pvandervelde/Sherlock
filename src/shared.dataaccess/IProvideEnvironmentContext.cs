//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Defines the interface for classes that provide access to stored environment data.
    /// </summary>
    public interface IProvideEnvironmentContext : IProvideStoredContext
    {
        /// <summary>
        /// Adds a new physical machine.
        /// </summary>
        /// <param name="machine">The new machine.</param>
        void Add(PhysicalMachineDescription machine);

        /// <summary>
        /// Adds a new Hyper-V machine.
        /// </summary>
        /// <param name="machine">The new machine.</param>
        void Add(HypervMachineDescription machine);

        /// <summary>
        /// Returns the machine with the given ID.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The desired machine.</returns>
        MachineDescription Machine(string id);

        /// <summary>
        /// Returns a collection containing all the available machines.
        /// </summary>
        /// <returns>A collection containing all the available machines.</returns>
        IEnumerable<MachineDescription> Machines();

        /// <summary>
        /// Returns a collection containing all the available physical machines.
        /// </summary>
        /// <returns>A collection containing all the available physical machines.</returns>
        IEnumerable<MachineDescription> PhysicalMachines();

        /// <summary>
        /// Returns a collection containing all the available Hyper-V machines.
        /// </summary>
        /// <returns>A collection containing all the available Hyper-V machines.</returns>
        IEnumerable<MachineDescription> HypervMachines();
            
        /// <summary>
        /// Returns a collection containing all the inactive machines that have the specified operating system
        /// and the specified set of applications.
        /// </summary>
        /// <param name="operatingSystem">The required operation system.</param>
        /// <param name="applications">The collection of required applications.</param>
        /// <returns>A collection containing all inactive machines with the specified operating system and the specified applications.</returns>
        IEnumerable<MachineDescription> InactiveMachinesWith(
            OperatingSystemDescription operatingSystem, 
            IEnumerable<ApplicationDescription> applications);

        /// <summary>
        /// Marks the given machine as active.
        /// </summary>
        /// <param name="machine">The ID of the machine.</param>
        void MarkMachineAsActive(string machine);

        /// <summary>
        /// Marks the given machine as inactive.
        /// </summary>
        /// <param name="machine">The ID of the machine.</param>
        void MarkMachineAsInactive(string machine);

        /// <summary>
        /// Updates the machine with the data from the given object.
        /// </summary>
        /// <param name="machine">The machine.</param>
        void Update(MachineDescription machine);

        /// <summary>
        /// Deletes an existing machine.
        /// </summary>
        /// <remarks>
        /// Deleting an machine may result in cascading deletes, e.g. when deleting a physical machine
        /// which is a host for one or more Hyper-V machines.
        /// </remarks>
        /// <param name="machine">The ID of the machine.</param>
        void DeleteMachine(string machine);

        /// <summary>
        /// Adds a new operating system.
        /// </summary>
        /// <param name="operatingSystem">The new operating system.</param>
        void Add(OperatingSystemDescription operatingSystem);

        /// <summary>
        /// Returns a collection containing all the available operating systems.
        /// </summary>
        /// <returns>A collection containing all the operating systems.</returns>
        IEnumerable<OperatingSystemDescription> OperatingSystems();

        /// <summary>
        /// Returns the operating system with the given ID.
        /// </summary>
        /// <param name="id">The operating system ID.</param>
        /// <returns>The requested operating system.</returns>
        OperatingSystemDescription OperatingSystem(int id);

        /// <summary>
        /// Updates the operating system with the data from the given object.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        void Update(OperatingSystemDescription operatingSystem);

        /// <summary>
        /// Deletes an existing operating system.
        /// </summary>
        /// <remarks>
        /// Deleting an operating system may result in cascading deletes elsewhere, e.g. when deleting an 
        /// operating system that is used by one or more machines.
        /// </remarks>
        /// <param name="id">The ID of the operating system.</param>
        void DeleteOperatingSystem(int id);

        /// <summary>
        /// Adds a new application.
        /// </summary>
        /// <param name="application">The new application.</param>
        void Add(ApplicationDescription application);

        /// <summary>
        /// Returns a collection containing all the available applications.
        /// </summary>
        /// <returns>A collection containing all the applications.</returns>
        IEnumerable<ApplicationDescription> Applications();

        /// <summary>
        /// Returns the application with the given ID.
        /// </summary>
        /// <param name="id">The application ID.</param>
        /// <returns>The requested application.</returns>
        ApplicationDescription Application(int id);

        /// <summary>
        /// Updates the application with the data from the given object.
        /// </summary>
        /// <param name="application">The application.</param>
        void Update(ApplicationDescription application);

        /// <summary>
        /// Deletes an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        void DeleteApplication(int id);
    }
}
