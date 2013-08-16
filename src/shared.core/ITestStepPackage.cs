//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that form a proxy for archive files that contain
    /// files for one or more tests.
    /// </summary>
    public interface ITestStepPackage : IPackageFile
    {
        /// <summary>
        /// Gets the index of the test step for which this package file stores the test files.
        /// </summary>
        int TestStepIndex
        {
            get;
        }

        /// <summary>
        /// Adds a new file to the package.
        /// </summary>
        /// <param name="fileToAdd">The full path of the new file that should be added.</param>
        /// <param name="packagedPath">The relative path of the file as stored in the package.</param>
        void Add(string fileToAdd, string packagedPath);

        /// <summary>
        /// Returns a collection containing all the file paths as stored in the package.
        /// </summary>
        /// <returns>A collection containing all the file paths as stored in the package.</returns>
        IEnumerable<FileInfo> StoredFiles();
    }
}
