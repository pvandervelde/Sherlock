//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the base interface for objects that handle packing and unpacking test files from an archive file.
    /// </summary>
    public interface IPackageFile
    {
        /// <summary>
        /// Gets the package path for the package.
        /// </summary>
        string PackagePath
        {
            get;
        }

        /// <summary>
        /// Extracts the information from the given package path and stores it for later use. Does not unpack
        /// the package file.
        /// </summary>
        /// <param name="packageFile">The full path to the package file.</param>
        void LoadFrom(string packageFile);

        /// <summary>
        /// Unpacks the test files from a given package file to a given destination location.
        /// </summary>
        /// <param name="destinationDirectory">The full path to the destination directory.</param>
        void UnpackTo(string destinationDirectory);

        /// <summary>
        /// Unpacks the test files from a given package file to a given destination location.
        /// </summary>
        /// <param name="packageFile">The full path to the package file.</param>
        /// <param name="destinationDirectory">The full path to the destination directory.</param>
        void LoadAndUnpack(string packageFile, string destinationDirectory);

        /// <summary>
        /// Packs the files into a package and stores it at the given package file path.
        /// </summary>
        /// <param name="file">The full path to the location where the new package file should be placed.</param>
        void PackTo(string file);
    }
}
