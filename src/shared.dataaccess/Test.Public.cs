//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API part for the test class.
    /// </content>
    public sealed partial class Test
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
        /// Gets the ID of the test.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_TestId;
            }
        }

        /// <summary>
        /// Gets a collection containing all the required test environments for the test.
        /// </summary>
        public IEnumerable<TestEnvironment> Environments
        {
            get
            {
                return TestEnvironments;
            }
        }
    }
}
