//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Web.Mvc;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Intranet.Models
{
    /// <summary>
    /// Defines the model of an operating system.
    /// </summary>
    public sealed class OperatingSystemModel
    {
        /// <summary>
        /// The object that handles the data storage.
        /// </summary>
        private readonly OperatingSystemDescription m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemModel"/> class.
        /// </summary>
        public OperatingSystemModel()
            : this(new OperatingSystemDescription())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemModel"/> class.
        /// </summary>
        /// <param name="model">The object that handles the data storage.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        public OperatingSystemModel(OperatingSystemDescription model)
        {
            {
                Lokad.Enforce.Argument(() => model);
            }

            m_Model = model;
        }

        /// <summary>
        /// Gets or sets the ID of the operating system.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int OperatingSystemId
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
        /// Gets or sets the name of the operating system.
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
        /// Gets or sets the description of the service pack of the operating system.
        /// </summary>
        public string ServicePack
        {
            get
            {
                return m_Model.ServicePack;
            }

            set
            {
                m_Model.ServicePack = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current operating system is used.
        /// </summary>
        public bool IsUsed
        {
            get
            {
                return m_Model.IsUsed;
            }
        }

        /// <summary>
        /// Gets or sets the pointer size for the operating system.
        /// </summary>
        public OperatingSystemPointerSize PointerSize
        {
            get
            {
                return m_Model.PointerSize;
            }

            set
            {
                m_Model.PointerSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the culture of the operating system.
        /// </summary>
        public string CultureInfo
        {
            get
            {
                return m_Model.CultureInfo.Name;
            }

            set
            {
                m_Model.CultureInfo = new CultureInfo(value);
            }
        }

        /// <summary>
        /// Returns the description that backs the current model.
        /// </summary>
        /// <returns>The description that backs the current model.</returns>
        internal OperatingSystemDescription ToDescription()
        {
            return m_Model;
        }
    }
}
