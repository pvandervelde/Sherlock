//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that form a proxy for archive files that contain
    /// files for a test environment.
    /// </summary>
    public interface ITestEnvironmentPackage : IPackageFile
    {
        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        string Environment
        {
            get;
        }

        /// <summary>
        /// Adds a new test step package to the current package.
        /// </summary>
        /// <param name="package">The package containing the data for a given test step.</param>
        void Add(ITestStepPackage package);

        /// <summary>
        /// Returns the archive for the test step at the given index.
        /// </summary>
        /// <param name="index">The index of the test step.</param>
        /// <returns>The archive for the test at the given index.</returns>
        ITestStepPackage Test(int index);

        /// <summary>
        /// Returns the collection of test step archives for the current environment.
        /// </summary>
        /// <returns>The collection of test step archives for the current environment.</returns>
        IEnumerable<ITestStepPackage> Tests();
    }
}
