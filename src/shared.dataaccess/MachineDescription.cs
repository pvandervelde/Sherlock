//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Defines the base class for machine environments.
    /// </summary>
    public abstract partial class MachineDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MachineDescription" /> class.
        /// </summary>
        public MachineDescription()
        {
            this.MachineApplications = new HashSet<MachineApplication>();
            this.HypervMachines = new HashSet<HypervMachineDescription>();
            this.TestEnvironments = new HashSet<TestEnvironment>();
        }
    
        /// <summary>
        /// Gets the ID of the machine environment.
        /// </summary>
        internal string pk_MachineId { get; set; }
    
        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        public string Name { get; set; }
    
        /// <summary>
        /// Gets or sets the description for the machine environment.
        /// </summary>
        public string Description { get; set; }
    
        /// <summary>
        /// Gets or sets the network name of the machine.
        /// </summary>
        public string NetworkName { get; set; }
    
        /// <summary>
        /// Gets or sets the MAC address of the machine.
        /// </summary>
        public string MacAddress { get; set; }
    
        /// <summary>
        /// Gets or sets a value indicating whether the current machine can be used for testing.
        /// </summary>
        public bool IsAvailableForTesting { get; set; }
    
        /// <summary>
        /// Gets or sets the ID of the operating system related to this machine.
        /// </summary>
        internal int fk_OperatingSystem { get; set; }
    
        /// <summary>
        /// Gets or sets a value indicating whether the machine is active or not.
        /// </summary>
        public bool IsActive { get; set; }
    
        /// <summary>
        /// Gets or sets the operating system for the current machine.
        /// </summary>
        public OperatingSystemDescription OperatingSystem { get; set; }
    
        /// <summary>
        /// Gets a collection containing all the applications installed on the current machine.
        /// </summary>
        public ICollection<MachineApplication> MachineApplications { get; internal set; }
    
        /// <summary>
        /// Gets a collection containing all the Hyper-V virtual machines that are hosted by the current machine.
        /// </summary>
        public ICollection<HypervMachineDescription> HypervMachines { get; internal set; }
    
        /// <summary>
        /// Gets the collection containing the test environments related to the current machine.
        /// </summary>
        public ICollection<TestEnvironment> TestEnvironments { get; internal set; }
    
    }
}
