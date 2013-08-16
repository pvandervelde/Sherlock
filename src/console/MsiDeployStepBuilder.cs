//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Xml.Linq;
using Sherlock.Console.Properties;

namespace Sherlock.Console
{
    /// <summary>
    /// Creates an MSI installing <see cref="TestStepDescription"/> from information stored in an XML configuration file.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Msi",
        Justification = "It's the MSI system we're dealing with here.")]
    internal sealed class MsiDeployStepBuilder : TestStepConstructor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsiDeployStepBuilder"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="fileStorage">The action that stores the files in a package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileStorage"/> is <see langword="null" />.
        /// </exception>
        public MsiDeployStepBuilder(IFileSystem fileSystem, StoreFileDataForEnvironment fileStorage) 
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
                return "msi";
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

            var file = (input.Element("file").FirstNode as XCData).Value;
            var parameters = GetParametersFromTestStepConfiguration(input);

            AddFileToEnvironmentPackage(environment, stepOrder, Path.GetFileName(file), file);

            return new MsiInstallTestStepDescription(environment, stepOrder, parameters);
        }
    }
}
