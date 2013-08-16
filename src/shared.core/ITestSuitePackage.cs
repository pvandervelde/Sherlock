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
    /// files for a test suite, consisting of one or more test environments.
    /// </summary>
    public interface ITestSuitePackage : IPackageFile
    {
        /// <summary>
        /// Adds the environment package to the current package.
        /// </summary>
        /// <param name="package">The package that contains the test files for a given environment.</param>
        void Add(ITestEnvironmentPackage package);

        /// <summary>
        /// Returns the archive for the environment with the given name.
        /// </summary>
        /// <param name="name">The name of the environment.</param>
        /// <returns>The archive for the environment with the given name.</returns>
        ITestEnvironmentPackage Environment(string name);

        /// <summary>
        /// Returns a collection containing all the environment archives for the current test suite.
        /// </summary>
        /// <returns>A collection containing all the environment archives for the current test suite.</returns>
        IEnumerable<ITestEnvironmentPackage> Environments();
    }
}
