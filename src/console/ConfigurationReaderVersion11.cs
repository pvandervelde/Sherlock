﻿//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using Sherlock.Console.Properties;

namespace Sherlock.Console
{
    /// <summary>
    /// An <see cref="IVersionedConfigurationReader"/> that reads configuration files with version 1.1.
    /// </summary>
    internal sealed class ConfigurationReaderVersion11 : ConfigurationReaderVersion1X
    {
        /// <summary>
        /// Gets the version of the configuration that the current reader can read.
        /// </summary>
        public static Version VersionToRead
        {
            get
            {
                return new Version(1, 1);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReaderVersion11"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="fileStorage">The action that stores the files in a package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileStorage"/> is <see langword="null" />.
        /// </exception>
        public ConfigurationReaderVersion11(
            IFileSystem fileSystem, 
            StoreFileDataForEnvironment fileStorage)
            : base(fileSystem, fileStorage)
        {
        }

        /// <summary>
        /// Determines whether this instance can read the given configuration with the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        ///   <see langword="true"/> if this instance can read the given configuration with the specified
        ///   version; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        protected override bool CanReadConfigurationWithVersion(Version version)
        {
            var assemblyVersion = VersionToRead;
            return (version.Major == assemblyVersion.Major) && (version.Minor == assemblyVersion.Minor);
        }

        /// <summary>
        /// Gets the version of the configuration file that is supported by the current reader.
        /// </summary>
        protected override Version SupportedConfigurationVersion
        {
            get
            {
                return VersionToRead;
            }
        }

        /// <summary>
        /// Extracts the files and directories that should be transferred back to the host after the 
        /// test step has completed.
        /// </summary>
        /// <param name="node">The node that contains the files and directories that should be transferred.</param>
        /// <returns>A value that indicates if the system log file should be transferred back to the host.</returns>
        protected override bool ExtractLogFileTransferFlagFromTestStepConfiguration(XElement node)
        {
            return false;
        }

        /// <summary>
        /// Extracts the files and directories that should be transferred back to the host after the 
        /// test step has completed.
        /// </summary>
        /// <param name="node">The node that contains the files and directories that should be transferred.</param>
        /// <returns>A collection containing the paths of all the files and directories that need to be copied back to the host.</returns>
        protected override IEnumerable<FileSystemInfo> ExtractFileElementsToCopyFromTestStepConfiguration(XElement node)
        {
            var transferElement = node.Element("includeinreport");
            if (transferElement != null)
            {
                throw new InvalidConfigurationFileException(
                    Resources.Exceptions_Messages_TransferTestStepElementNotValidInCurrentConfigurationVersion);
            }

            return Enumerable.Empty<FileSystemInfo>();
        }
    }
}
