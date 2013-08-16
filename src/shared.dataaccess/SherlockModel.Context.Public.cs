//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API for the Sherlock Context class.
    /// </content>
    internal sealed partial class SherlockContext : IProvideTestingContext
    {
        /// <summary>
        /// Initializes the context.
        /// </summary>
        partial void Initialize()
        {
            VerifySchemaVersion();

            // Initialize the storage before we initialize the actual
            // objects in the storage, because as we're adding the database objects
            // we may need to access storage objects from a location we haven't initialized yet, i.e.
            // while initializing environments we need access to the test collections.
            InitializeStorage();
            InitializeEnvironments();
            InitializeTests();
        }

        private void InitializeStorage()
        {
            StoredApplicationDescriptions = Set<ApplicationDescription>();
            StoredOperatingSystemDescriptions = Set<OperatingSystemDescription>();
            StoredMachineDescriptions = Set<MachineDescription>();
            StoredMachineApplications = Set<MachineApplication>();

            StoredTestApplications = Set<TestApplication>();
            StoredTestEnvironments = Set<TestEnvironment>();
            StoredTestStepParameters = Set<TestStepParameter>();
            StoredTestSteps = Set<TestStep>();
            StoredTests = Set<Test>();
        }

        private void VerifySchemaVersion()
        {
            var version = GetSchemaVersion().FirstOrDefault();
            if ((!version.HasValue) || (version != DataAccessConstants.ExpectedSchemaVersion))
            {
                throw new InvalidSchemaVersionException(DataAccessConstants.ExpectedSchemaVersion, version.Value);
            }
        }

        /// <summary>
        /// Stores all the changes via the underlying storage mechanism.
        /// </summary>
        public void StoreChanges()
        {
            VerifySchemaVersion();
            SaveChanges();
        }
    }
}
