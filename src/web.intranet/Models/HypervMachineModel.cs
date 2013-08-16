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
    /// Defines the model of a Hyper-V machine.
    /// </summary>
    public sealed class HypervMachineModel : MachineModel
    {
        /// <summary>
        /// The object that handles the data storage.
        /// </summary>
        private readonly HypervMachineDescription m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="HypervMachineModel"/> class.
        /// </summary>
        public HypervMachineModel()
            : this(new HypervMachineDescription())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HypervMachineModel"/> class.
        /// </summary>
        /// <param name="model">The object that handles the data storage.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        public HypervMachineModel(HypervMachineDescription model)
            : base(model)
        {
            {
                Lokad.Enforce.Argument(() => model);
            }

            m_Model = model;
        }

        /// <summary>
        /// Gets or sets the ID of the host machine.
        /// </summary>
        public string HostMachineId
        {
            get
            {
                return m_Model.HostMachineId;
            }

            set
            {
                m_Model.HostMachineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the virtual machine image.
        /// </summary>
        public string Image
        {
            get
            {
                return m_Model.Image;
            }

            set
            {
                m_Model.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the snapshot to which the virtual machine should be returned after use.
        /// </summary>
        public string SnapshotToReturnTo
        {
            get
            {
                return m_Model.SnapshotToReturnTo;
            }

            set
            {
                m_Model.SnapshotToReturnTo = value;
            }
        }

        /// <summary>
        /// Returns the description that backs the current model.
        /// </summary>
        /// <returns>The description that backs the current model.</returns>
        public HypervMachineDescription ToDescription()
        {
            return m_Model;
        }
    }
}
