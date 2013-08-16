//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the base methods for classes that create <see cref="TestStepDescription"/> objects from an XML configuration filePath.
    /// </summary>
    internal abstract class TestStepConstructor : IConstructTestSteps
    {
        /// <summary>
        /// Extracts the test step name from the configuration element.
        /// </summary>
        /// <param name="input">The configuration element for the current test step.</param>
        /// <returns>The name of the test step in the overall test step sequence.</returns>
        protected static int GetStepOrderFromTestStepConfiguration(XElement input)
        {
            return int.Parse(input.Attribute("steporder").Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Extracts the environment name from the configuration element.
        /// </summary>
        /// <param name="input">The configuration element for the current test step.</param>
        /// <returns>The name of the environment that will execute the current test step.</returns>
        protected static string GetEnvironmentNameFromTestStepConfiguration(XElement input)
        {
            return input.Attribute("environment").Value;
        }

        /// <summary>
        /// Extracts a collection of parameters from the configuration element.
        /// </summary>
        /// <param name="input">The configuration element for the current test step.</param>
        /// <returns>A collection containing all the parameters.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Users should really be putting in XElement objects.")]
        protected static IEnumerable<TestStepParameterDescription> GetParametersFromTestStepConfiguration(XElement input)
        {
            var parameters = new List<TestStepParameterDescription>();
            foreach (var element in input.Element("params").Elements("param"))
            {
                var key = element.Attribute("key").Value;
                var value = (element.FirstNode as XCData).Value;

                parameters.Add(new TestStepParameterDescription(key, value));
            }

            return parameters;
        }

        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The action that stores a filePath into a package.
        /// </summary>
        private readonly StoreFileDataForEnvironment m_FileStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepConstructor"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="fileStorage">The action that stores the files in a package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileStorage"/> is <see langword="null" />.
        /// </exception>
        protected TestStepConstructor(IFileSystem fileSystem, StoreFileDataForEnvironment fileStorage)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => fileStorage);
            }

            m_FileSystem = fileSystem;
            m_FileStorage = fileStorage;
        }

        /// <summary>
        /// Gets the object that provides access to the file system.
        /// </summary>
        protected IFileSystem FileSystem
        {
            get
            {
                return m_FileSystem;
            }
        }

        /// <summary>
        /// Adds the given filePath to the ZIP-package that contains all the files for the current environment.
        /// </summary>
        /// <param name="environment">The name of the environment.</param>
        /// <param name="stepOrder">The name of the test step.</param>
        /// <param name="storedPath">The path that is stored in the package.</param>
        /// <param name="filePath">The full path to the filePath.</param>
        protected void AddFileToEnvironmentPackage(string environment, int stepOrder, string storedPath, string filePath)
        {
            m_FileStorage(environment, stepOrder, storedPath, filePath);
        }

        /// <summary>
        /// Gets the contract that indicates what kind of input data this constructor can
        /// process.
        /// </summary>
        public abstract string Contract
        {
            get;
        }

        /// <summary>
        /// Constructs a test step from the given data.
        /// </summary>
        /// <param name="input">The XML element that contains the data for the test step.</param>
        /// <returns>The newly created test step.</returns>
        public abstract TestStepDescription Construct(XElement input);
    }
}
