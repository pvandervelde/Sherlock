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
    /// Defines the model of a physical machine.
    /// </summary>
    public sealed class PhysicalMachineModel : MachineModel
    {
        /// <summary>
        /// The object that handles the data storage.
        /// </summary>
        private readonly PhysicalMachineDescription m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalMachineModel"/> class.
        /// </summary>
        public PhysicalMachineModel()
            : this(new PhysicalMachineDescription())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalMachineModel"/> class.
        /// </summary>
        /// <param name="model">The object that handles the data storage.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        public PhysicalMachineModel(PhysicalMachineDescription model)
            : base(model)
        {
            {
                Lokad.Enforce.Argument(() => model);
            }

            m_Model = model;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current machine can be started via a remote network call.
        /// </summary>
        public bool CanStartRemotely
        {
            get
            {
                return m_Model.CanStartRemotely;
            }

            set
            {
                m_Model.CanStartRemotely = value;
            }
        }

        /// <summary>
        /// Returns the description that backs the current model.
        /// </summary>
        /// <returns>The description that backs the current model.</returns>
        public PhysicalMachineDescription ToDescription()
        {
            return m_Model;
        }
    }
}
