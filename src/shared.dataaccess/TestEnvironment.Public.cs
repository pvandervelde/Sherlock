//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API part for the test environment class.
    /// </content>
    public sealed partial class TestEnvironment
    {
        /// <summary>
        /// Gets or sets a value indicating whether the object is currently undergoing patching, i.e.
        /// adding all values from the database.
        /// </summary>
        internal bool IsPatching
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object has been patched, i.e. all values have been
        /// extracted from the database, or not.
        /// </summary>
        internal bool IsPatched
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ID of the test environment.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_TestEnvironmentId;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the test linked to the current environment.
        /// </summary>
        public int TestId
        {
            get
            {
                return fk_TestId;
            }

            set
            {
                fk_TestId = value;
            }
        }

        /// <summary>
        /// Gets or sets the operating system desired for the current environment.
        /// </summary>
        public int DesiredOperatingSystemId
        {
            get
            {
                return fk_DesiredOperatingSystemId;
            }

            set
            {
                fk_DesiredOperatingSystemId = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the machine selected as backing for the current environment.
        /// </summary>
        public string SelectedMachineId
        {
            get
            {
                return fk_SelectedMachineId;
            }

            set
            {
                fk_SelectedMachineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        public string Name
        {
            get
            {
                return EnvironmentName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException();
                }

                EnvironmentName = value;
            }
        }

        /// <summary>
        /// Gets a collection containing all the required applications for the current environment.
        /// </summary>
        public IEnumerable<ApplicationDescription> Applications
        {
            get
            {
                return TestApplications.Select(a => a.Application);
            }
        }
    }
}
