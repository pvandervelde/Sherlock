//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Intranet.Models
{
    /// <summary>
    /// Defines the model of an application.
    /// </summary>
    public sealed class ApplicationModel
    {
        /// <summary>
        /// The object that handles the data storage.
        /// </summary>
        private readonly ApplicationDescription m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationModel"/> class.
        /// </summary>
        public ApplicationModel()
            : this(new ApplicationDescription())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationModel"/> class.
        /// </summary>
        /// <param name="model">The object that handles the data storage.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        public ApplicationModel(ApplicationDescription model)
        {
            {
                Lokad.Enforce.Argument(() => model);
            }

            m_Model = model;
        }

        /// <summary>
        /// Gets or sets the ID of the application.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int ApplicationId
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
        /// Gets or sets the name of the application.
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
        /// Gets or sets the version of the application.
        /// </summary>
        public string Version
        {
            get
            {
                return m_Model.Version.ToString();
            }

            set
            {
                m_Model.Version = new Version(value);
            }
        }

        /// <summary>
        /// Returns the description that backs the current model.
        /// </summary>
        /// <returns>The description that backs the current model.</returns>
        public ApplicationDescription ToDescription()
        {
            return m_Model;
        }
    }
}
