//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// The description of a test and the product that is being tested.
    /// </summary>
    internal sealed class TestDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDescription"/> class.
        /// </summary>
        /// <param name="productUnderTest">The name of the product that is being tested.</param>
        /// <param name="versionOfProductUnderTest">The version of the product that is being tested.</param>
        /// <param name="owner">The name of the user who requested the test.</param>
        /// <param name="description">The description of the test.</param>
        /// <param name="reportPath">The full path to the location where the report should be placed.</param>
        /// <param name="environments">The collection of environments required for the test.</param>
        /// <param name="testSteps">The collection of test steps for the test.</param>
        public TestDescription(
            string productUnderTest, 
            string versionOfProductUnderTest, 
            string owner, 
            string description,
            string reportPath,
            IEnumerable<TestEnvironmentDescription> environments,
            IEnumerable<TestStepDescription> testSteps)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(reportPath), "The report path should not be an empty string.");

                Debug.Assert(environments != null, "The environments collection should not be a null reference.");
                Debug.Assert(environments.Any(), "The environments collection should not be empty.");

                Debug.Assert(testSteps != null, "The test steps collection should not be a null reference.");
                Debug.Assert(testSteps.Any(), "The test steps collection should not be empty.");
            }

            ProductUnderTest = productUnderTest;
            VersionOfProductUnderTest = versionOfProductUnderTest;
            Owner = owner;
            Description = description;
            ReportPath = reportPath;
            Environments = environments;
            TestSteps = testSteps;
        }

        /// <summary>
        /// Gets or sets the assigned ID of the test.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the product that is being tested.
        /// </summary>
        public string ProductUnderTest
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version of the product that is being tested.
        /// </summary>
        public string VersionOfProductUnderTest
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the user who requested the test.
        /// </summary>
        public string Owner
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of the test.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the full path to the location where the report should be placed.
        /// </summary>
        public string ReportPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection containing the descriptions for all the environments required for the test.
        /// </summary>
        public IEnumerable<TestEnvironmentDescription> Environments
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection containing the test steps for the test.
        /// </summary>
        public IEnumerable<TestStepDescription> TestSteps
        {
            get;
            private set;
        }
    }
}
