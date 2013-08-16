//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using System.Xml.Linq;
using Sherlock.Console.Properties;

namespace Sherlock.Console
{
    /// <summary>
    /// Creates an x-copy deployment <see cref="TestStepDescription"/> from information stored in an XML configuration file.
    /// </summary>
    internal sealed class XCopyDeployStepBuilder : TestStepConstructor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XCopyDeployStepBuilder"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="fileStorage">The action that stores the files in a package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileStorage"/> is <see langword="null" />.
        /// </exception>
        public XCopyDeployStepBuilder(IFileSystem fileSystem, StoreFileDataForEnvironment fileStorage) 
            : base(fileSystem, fileStorage)
        {
        }

        /// <summary>
        /// Gets the contract that indicates what kind of input data this constructor can
        /// process.
        /// </summary>
        public override string Contract
        {
            get
            {
                return "xcopy";
            }
        }

        /// <summary>
        /// Constructs a test step from the given data.
        /// </summary>
        /// <param name="input">The XML element that contains the data for the test step.</param>
        /// <returns>The newly created test step.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="input"/> does not have the correct contract name.
        /// </exception>
        public override TestStepDescription Construct(XElement input)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    string.Equals(Contract, input.Name.LocalName, StringComparison.Ordinal),
                    Resources.Exceptions_Messages_InvalidInputElement);
            }

            var stepOrder = GetStepOrderFromTestStepConfiguration(input);
            var environment = GetEnvironmentNameFromTestStepConfiguration(input);

            var remoteBasePath = (input.Element("destination").FirstNode as XCData).Value;
            var basePath = (input.Element("base").FirstNode as XCData).Value;

            foreach (var element in input.Element("paths").Elements())
            {
                var path = (element.FirstNode as XCData).Value;
                if (string.Equals("file", element.Name.LocalName, StringComparison.Ordinal))
                {
                    var relativePath = path.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
                    AddFileToEnvironmentPackage(environment, stepOrder, relativePath, path);
                }

                if (string.Equals("directory", element.Name.LocalName, StringComparison.Ordinal))
                {
                    foreach (var file in FileSystem.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                    {
                        var relativePath = file.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
                        AddFileToEnvironmentPackage(environment, stepOrder, relativePath, file);
                    }
                }
            }

            return new XCopyTestStepDescription(environment, stepOrder, remoteBasePath);
        }
    }
}
