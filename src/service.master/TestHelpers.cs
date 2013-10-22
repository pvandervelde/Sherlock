//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.IO.Abstractions;
using Nuclei.Configuration;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Provides helper methods for dealing with tests.
    /// </summary>
    internal static class TestHelpers
    {
        /// <summary>
        /// Returns the full path to the file that contains all the input file data for a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <param name="configuration">The object that stores the configuration for the current application.</param>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <returns>The full path to the package file that contains all the input file data for the given test.</returns>
        public static string StoragePathForTestFiles(int test, IConfiguration configuration, IFileSystem fileSystem)
        {
            var testFile = fileSystem.Path.Combine(
                configuration.Value<string>(MasterServiceConfigurationKeys.TestDataDirectory),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.zip",
                    test));

            return testFile;
        }

        /// <summary>
        /// Returns the full path to the directory in which all the test report files are placed.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <param name="configuration">The object that stores the configuration for the current application.</param>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <returns>The full path to the package file that contains all the input file data for the given test.</returns>
        public static string StoragePathForReportFiles(int test, IConfiguration configuration, IFileSystem fileSystem)
        {
            var reportFileDirectory = 
                fileSystem.Path.Combine(
                    fileSystem.Path.Combine(
                        configuration.Value<string>(MasterServiceConfigurationKeys.TestReportFilesDirectory),
                        "reportfiles"),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}",
                        test));

            return reportFileDirectory;
        }
    }
}
