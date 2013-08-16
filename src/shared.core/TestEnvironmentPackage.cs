//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Sherlock.Shared.Core.Properties;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Provides proxy access to an archive file that contains the archives for one or more tests
    /// on a given environment.
    /// </summary>
    public sealed class TestEnvironmentPackage : ITestEnvironmentPackage
    {
        /// <summary>
        /// The file name of the meta data file that describes the current package.
        /// </summary>
        private const string MetaDataFileName = @".meta";

        /// <summary>
        /// The collection containing all the test steps for the current environment.
        /// </summary>
        private readonly SortedList<int, ITestStepPackage> m_TestSteps
            = new SortedList<int, ITestStepPackage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironmentPackage"/> class.
        /// </summary>
        public TestEnvironmentPackage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironmentPackage"/> class.
        /// </summary>
        /// <param name="name">The name of the environment.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public TestEnvironmentPackage(string name)
            : this()
        {
            {
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);
            }

            Environment = name;
        }

        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        public string Environment
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds a new test step package to the current package.
        /// </summary>
        /// <param name="package">The package containing the data for a given test step.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="package"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if a package with the same index as <paramref name="package"/> has 
        ///     already been stored.
        /// </exception>
        public void Add(ITestStepPackage package)
        {
            {
                Lokad.Enforce.Argument(() => package);
                Lokad.Enforce.With<ArgumentException>(
                    !m_TestSteps.ContainsKey(package.TestStepIndex),
                    Resources.Exceptions_Messages_CannotAddDuplicatePackage);
            }

            m_TestSteps.Add(package.TestStepIndex, package);
        }

        /// <summary>
        /// Returns the archive for the test step at the given index.
        /// </summary>
        /// <param name="index">The index of the test step.</param>
        /// <returns>The archive for the test at the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="index"/> is either small than zero or larger
        ///     than the largest value in the collection.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if no package with the given <paramref name="index"/> has 
        ///     been stored.
        /// </exception>
        public ITestStepPackage Test(int index)
        {
            return !m_TestSteps.ContainsKey(index) ? null : m_TestSteps[index];
        }

        /// <summary>
        /// Returns the collection of test step archives for the current environment.
        /// </summary>
        /// <returns>The collection of test step archives for the current environment.</returns>
        public IEnumerable<ITestStepPackage> Tests()
        {
            return m_TestSteps.Values;
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
            using (var zipFile = ZipFile.Read(PackagePath))
            {
                foreach (var entry in zipFile)
                {
                    if (string.Equals(MetaDataFileName, entry.FileName))
                    {
                        ExtractEnvironmentName(entry);
                        break;
                    }
                }
            }
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
                    if (string.Equals(MetaDataFileName, entry.FileName))
                    {
                        ExtractEnvironmentName(entry);
                        continue;
                    }

                    entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);

                    var step = new TestStepPackage();
                    step.LoadFrom(Path.Combine(destinationDirectory, entry.FileName));
                    m_TestSteps.Add(step.TestStepIndex, step);
                }
            }
        }

        private void ExtractEnvironmentName(ZipEntry entry)
        {
            using (var reader = new StreamReader(entry.OpenReader()))
            {
                Environment = reader.ReadToEnd();
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

                zipFile.AddEntry(MetaDataFileName, Environment);
                foreach (var pair in m_TestSteps)
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
