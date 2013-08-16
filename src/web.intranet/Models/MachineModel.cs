//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Intranet.Models
{
    /// <summary>
    /// Defines the model of a machine.
    /// </summary>
    public abstract class MachineModel
    {
        /// <summary>
        /// The object that handles the data storage.
        /// </summary>
        private readonly MachineDescription m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineModel"/> class.
        /// </summary>
        /// <param name="model">The object that handles the data storage.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        protected MachineModel(MachineDescription model)
        {
            {
                Lokad.Enforce.Argument(() => model);
            }

            m_Model = model;
        }

        /// <summary>
        /// Gets or sets the ID of the machine.
        /// </summary>
        public string MachineId
        {
            get
            {
                return m_Model.Id;
            }

            set
            {
                m_Model.Id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Model.Name;
            }

            set
            {
                m_Model.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the description for the machine environment.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Model.Description;
            }

            set
            {
                m_Model.Description = value;
            }
        }

        /// <summary>
        /// Gets or sets the network name of the machine.
        /// </summary>
        public string NetworkName
        {
            get
            {
                return m_Model.NetworkName;
            }

            set
            {
                m_Model.NetworkName = value;
            }
        }

        /// <summary>
        /// Gets or sets the MAC address of the machine.
        /// </summary>
        public string MacAddress
        {
            get
            {
                return m_Model.MacAddress;
            }

            set
            {
                m_Model.MacAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the operating system related to this machine.
        /// </summary>
        public int OperatingSystemId
        {
            get
            {
                return m_Model.OperatingSystemId;
            }

            set
            {
                m_Model.OperatingSystemId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current machine can be used for testing.
        /// </summary>
        public bool IsAvailableForTesting
        {
            get
            {
                return m_Model.IsAvailableForTesting;
            }

            set
            {
                m_Model.IsAvailableForTesting = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the machine is active or not.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return m_Model.IsActive;
            }

            set
            {
                m_Model.IsActive = value;
            }
        }
    }
}
