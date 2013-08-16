//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Ionic.Zip;
using Sherlock.Shared.Core.Properties;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Provides proxy access to an archive file that contains the archives for one or more environments
    /// in a given test.
    /// </summary>
    public sealed class TestSuitePackage : ITestSuitePackage
    {
        /// <summary>
        /// The collection containing all the zip archives for the different environments for the
        /// current test.
        /// </summary>
        private readonly Dictionary<string, ITestEnvironmentPackage> m_TestEnvironments
            = new Dictionary<string, ITestEnvironmentPackage>();

        /// <summary>
        /// Adds the environment package to the current package.
        /// </summary>
        /// <param name="package">The package that contains the test files for a given environment.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="package"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if a package with the same name as <paramref name="package"/> has 
        ///     already been stored.
        /// </exception>
        public void Add(ITestEnvironmentPackage package)
        {
            {
                Lokad.Enforce.Argument(() => package);
                Lokad.Enforce.With<ArgumentException>(
                    !m_TestEnvironments.ContainsKey(package.Environment),
                    Resources.Exceptions_Messages_CannotAddDuplicatePackage);
            }

            m_TestEnvironments.Add(package.Environment, package);
        }

        /// <summary>
        /// Returns the archive for the environment with the given name.
        /// </summary>
        /// <param name="name">The name of the environment.</param>
        /// <returns>The archive for the environment with the given name.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        public ITestEnvironmentPackage Environment(string name)
        {
            {
                Lokad.Enforce.Argument(() => name);
            }

            return !m_TestEnvironments.ContainsKey(name) ? null : m_TestEnvironments[name];
        }

        /// <summary>
        /// Returns a collection containing all the environment archives for the current test suite.
        /// </summary>
        /// <returns>A collection containing all the environment archives for the current test suite.</returns>
        public IEnumerable<ITestEnvironmentPackage> Environments()
        {
            return m_TestEnvironments.Values;
        }

        /// <summary>
        /// Gets the package path for the package.
        /// </summary>
        public string PackagePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Extracts the information from the given package path and stores it for later use. Does not unpack
        /// the package file.
        /// </summary>
        /// <param name="packageFile">The full path to the package file.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageFile"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="packageFile"/> is an empty string.
        /// </exception>
        public void LoadFrom(string packageFile)
        {
            {
                Lokad.Enforce.Argument(() => packageFile);
                Lokad.Enforce.Argument(() => packageFile, Lokad.Rules.StringIs.NotEmpty);
            }

            PackagePath = packageFile;
        }

        /// <summary>
        /// Unpacks the test files from a given package file to a given destination location.
        /// </summary>
        /// <param name="destinationDirectory">The full path to the destination directory.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="destinationDirectory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="destinationDirectory"/> is an empty string.
        /// </exception>
        /// <exception cref="NeedPackagePathException">
        ///     Thrown if no <see cref="PackagePath"/> has been provided.
        /// </exception>
        public void UnpackTo(string destinationDirectory)
        {
            {
                Lokad.Enforce.Argument(() => destinationDirectory);
                Lokad.Enforce.Argument(() => destinationDirectory, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.With<NeedPackagePathException>(
                    !string.IsNullOrWhiteSpace(PackagePath),
                    Resources.Exceptions_Messages_NeedPackagePath);
            }

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            using (var zipFile = ZipFile.Read(PackagePath))
            {
                foreach (var entry in zipFile)
                {
                    entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);

                    var step = new TestEnvironmentPackage();
                    step.LoadFrom(Path.Combine(destinationDirectory, entry.FileName));
                    m_TestEnvironments.Add(step.Environment, step);
                }
            }
        }

        /// <summary>
        /// Unpacks the test files from a given package file to a given destination location.
        /// </summary>
        /// <param name="packageFile">The full path to the package file.</param>
        /// <param name="destinationDirectory">The full path to the destination directory.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageFile"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="packageFile"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="destinationDirectory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="destinationDirectory"/> is an empty string.
        /// </exception>
        public void LoadAndUnpack(string packageFile, string destinationDirectory)
        {
            {
                Lokad.Enforce.Argument(() => packageFile);
                Lokad.Enforce.Argument(() => packageFile, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => destinationDirectory);
                Lokad.Enforce.Argument(() => destinationDirectory, Lokad.Rules.StringIs.NotEmpty);
            }

            PackagePath = packageFile;
            UnpackTo(destinationDirectory);
        }

        /// <summary>
        /// Packs the files into a package and stores it at the given package file path.
        /// </summary>
        /// <param name="file">The full path to the location where the new package file should be placed.</param>
        public void PackTo(string file)
        {
            var zipFile = !File.Exists(file) ? new ZipFile(file) : ZipFile.Read(file);
            using (zipFile)
            {
                var directory = Path.GetDirectoryName(file);

                foreach (var pair in m_TestEnvironments)
                {
                    var packageFile = Path.Combine(
                        directory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.zip",
                            pair.Key));
                    pair.Value.PackTo(packageFile);
                    zipFile.AddFile(packageFile).FileName = Path.GetFileName(packageFile);
                }

                zipFile.Save();
            }
        }
    }
}
