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
    /// Provides proxy access to an archive file that contains one or more files for a test.
    /// </summary>
    public sealed class TestStepPackage : ITestStepPackage
    {
        /// <summary>
        /// The file name of the meta data file that describes the current package.
        /// </summary>
        private const string MetaDataFileName = @".meta";

        /// <summary>
        /// The collection mapping all the test files to their storage location.
        /// </summary>
        private readonly Dictionary<string, string> m_Files
            = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepPackage"/> class.
        /// </summary>
        internal TestStepPackage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepPackage"/> class.
        /// </summary>
        /// <param name="index">The index of the test step.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="index"/> is <see langword="null" />.
        /// </exception>
        public TestStepPackage(int index)
            : this()
        {
            {
                Lokad.Enforce.With<ArgumentOutOfRangeException>(
                    index >= 0,
                    Resources.Exceptions_Messages_ArgumentOutOfRange);
            }

            TestStepIndex = index;
        }

        /// <summary>
        /// Gets the index of the test step for which this package file stores the test files.
        /// </summary>
        public int TestStepIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds a new file to the package.
        /// </summary>
        /// <param name="fileToAdd">The full path of the new file that should be added.</param>
        /// <param name="packagedPath">The relative path of the file as stored in the package.</param>
        public void Add(string fileToAdd, string packagedPath)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    !m_Files.ContainsKey(fileToAdd),
                    Resources.Exceptions_Messages_CannotAddDuplicateFileToPackage);
                Lokad.Enforce.With<ArgumentException>(
                    !m_Files.ContainsValue(packagedPath),
                    Resources.Exceptions_Messages_CannotAddDuplicateFileToPackage);
            }

            m_Files.Add(fileToAdd, packagedPath);
        }

        /// <summary>
        /// Returns a collection containing all the file paths as stored in the package.
        /// </summary>
        /// <returns>A collection containing all the file paths as stored in the package.</returns>
        public IEnumerable<FileInfo> StoredFiles()
        {
            return m_Files.Select(p => new FileInfo(p.Key));
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
                        ExtractTestStepIndex(entry);
                        break;
                    }
                }
            }
        }

        private void ExtractTestStepIndex(ZipEntry entry)
        {
            using (var reader = new StreamReader(entry.OpenReader()))
            {
                var text = reader.ReadToEnd();
                TestStepIndex = int.Parse(text);
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
                        ExtractTestStepIndex(entry);
                        continue;
                    }

                    m_Files.Add(Path.Combine(destinationDirectory, entry.FileName), entry.FileName);
                    entry.Extract(destinationDirectory, ExtractExistingFileAction.OverwriteSilently);
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
                zipFile.AddEntry(MetaDataFileName, TestStepIndex.ToString(CultureInfo.InvariantCulture));
                foreach (var pair in m_Files)
                {
                    zipFile.AddFile(pair.Key).FileName = pair.Value;
                }

                zipFile.Save();
            }
        }
    }
}
