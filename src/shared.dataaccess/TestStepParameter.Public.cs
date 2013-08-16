//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API part for the test step parameter class.
    /// </content>
    [Serializable]
    public sealed partial class TestStepParameter
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
        /// Gets the ID of the test step parameter.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_TestStepParameterId;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the test step that the current parameter is linked to.
        /// </summary>
        public int TestStepId
        {
            get
            {
                return fk_TestStepId;
            }

            set
            {
                fk_TestStepId = value;
            }
        }
    }
}
