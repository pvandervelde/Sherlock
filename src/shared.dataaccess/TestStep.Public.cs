//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API part for the test step class.
    /// </content>
    [Serializable]
    public abstract partial class TestStep
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
        /// Gets the ID for the test step.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_TestStepId;
            }
        }

        /// <summary>
        /// Gets or sets the failure mode for the current test step.
        /// </summary>
        public TestStepFailureMode FailureMode
        {
            get
            {
                return (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), OnFailure);
            }

            set
            {
                OnFailure = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the ID of the test environment to which the current test step belongs.
        /// </summary>
        public int TestEnvironmentId
        {
            get
            {
                return fk_TestEnvironmentId;
            }

            set
            {
                fk_TestEnvironmentId = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection containing the parameters for the current test step.
        /// </summary>
        public IEnumerable<TestStepParameter> Parameters
        {
            get
            {
                return TestStepParameters;
            }

            set
            {
                TestStepParameters.Clear();
                if (value != null)
                {
                    foreach (var parameter in value)
                    {
                        TestStepParameters.Add(parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection containing the directories that should be added to the 
        /// report upon completion of the current test step.
        /// </summary>
        public IEnumerable<TestStepReportDirectory> ReportDirectories
        {
            get
            {
                return TestStepReportDirectories;
            }

            set
            {
                TestStepReportDirectories.Clear();
                if (value != null)
                {
                    foreach (var directory in value)
                    {
                        TestStepReportDirectories.Add(directory);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection containing the files that should be added to the 
        /// report upon completion of the current test step.
        /// </summary>
        public IEnumerable<TestStepReportFile> ReportFiles
        {
            get
            {
                return TestStepReportFiles;
            }

            set
            {
                TestStepReportFiles.Clear();
                if (value != null)
                {
                    foreach (var file in value)
                    {
                        TestStepReportFiles.Add(file);
                    }
                }
            }
        }
    }
}
