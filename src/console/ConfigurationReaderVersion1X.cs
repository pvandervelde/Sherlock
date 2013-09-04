//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// The base <see cref="IVersionedConfigurationReader"/> that reads configuration files with version 1.X.
    /// </summary>
    internal abstract class ConfigurationReaderVersion1X : IVersionedConfigurationReader
    {
        /// <summary>
        /// Extracts the test step name from the configuration element.
        /// </summary>
        /// <param name="input">The configuration element for the current test step.</param>
        /// <returns>The name of the test step in the overall test step sequence.</returns>
        private static int ExtractStepOrderFromTestStepConfiguration(XElement input)
        {
            return int.Parse(input.Attribute("steporder").Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Extracts the environment name from the configuration element.
        /// </summary>
        /// <param name="input">The configuration element for the current test step.</param>
        /// <returns>The name of the environment that will execute the current test step.</returns>
        private static string ExtractEnvironmentNameFromTestStepConfiguration(XElement input)
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
        private static IEnumerable<TestStepParameterDescription> ExtractParametersFromTestStepConfiguration(XElement input)
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
        /// Initializes a new instance of the <see cref="ConfigurationReaderVersion1X"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="fileStorage">The action that stores the files in a package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileStorage"/> is <see langword="null" />.
        /// </exception>
        protected ConfigurationReaderVersion1X(IFileSystem fileSystem, StoreFileDataForEnvironment fileStorage)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => fileStorage);
            }

            m_FileSystem = fileSystem;
            m_FileStorage = fileStorage;
        }

        /// <summary>
        /// Adds the given filePath to the ZIP-package that contains all the files for the current environment.
        /// </summary>
        /// <param name="environment">The name of the environment.</param>
        /// <param name="stepOrder">The name of the test step.</param>
        /// <param name="storedPath">The path that is stored in the package.</param>
        /// <param name="filePath">The full path to the filePath.</param>
        private void AddFileToEnvironmentPackage(string environment, int stepOrder, string storedPath, string filePath)
        {
            m_FileStorage(environment, stepOrder, storedPath, filePath);
        }

        /// <summary>
        /// Reads the configuration from the XML document.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>A new configuration information object.</returns>
        public ConfigurationInfo Read(XDocument xmlDocument)
        {
            var rootNode = xmlDocument.Element("sherlock");

            var configurationVersion = new Version(rootNode.Attribute("configurationVersion").Value);
            if (!CanReadConfigurationWithVersion(configurationVersion))
            {
                throw new NonMatchingVersionFoundException(configurationVersion.ToString(), SupportedConfigurationVersion.ToString());
            }

            var descriptionNode = rootNode.Element("description");
            var productName = descriptionNode.Element("product").Value;
            var productVersion = descriptionNode.Element("version").Value;
            var testPurpose = descriptionNode.Element("testpurpose").Value;
            var owner = string.Format(
                CultureInfo.InvariantCulture,
                @"{0}\{1}",
                Environment.UserDomainName,
                Environment.UserName);

            var constraints = ExtractEnvironmentConstraints(rootNode);
            var testSteps = ExtractTestSteps(rootNode);
            var reportPath = ExtractCompletedNotification(rootNode);

            var description = new TestDescription(productName, productVersion, owner, testPurpose, reportPath, constraints, testSteps);
            return new ConfigurationInfo(description);
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
        protected abstract bool CanReadConfigurationWithVersion(Version version);

        /// <summary>
        /// Gets the version of the configuration file that is supported by the current reader.
        /// </summary>
        protected abstract Version SupportedConfigurationVersion
        {
            get;
        }

        private IEnumerable<TestEnvironmentDescription> ExtractEnvironmentConstraints(XElement rootNode)
        {
            var result = new List<TestEnvironmentDescription>();

            var environmentConstraintNodes = rootNode.Element("environments").Elements("environment");
            foreach (var environmentNode in environmentConstraintNodes)
            {
                var name = environmentNode.Attribute("name").Value;
                var constraintNodes = environmentNode.Element("constraints");

                OperatingSystemDescription operatingSystem = null;
                var applications = new List<ApplicationDescription>();
                foreach (var constraintNode in constraintNodes.Elements())
                {
                    if (string.Equals("operatingsystem", constraintNode.Name.LocalName, StringComparison.Ordinal))
                    {
                        operatingSystem = ExtractOperatingSystemConstraint(constraintNode);
                    }

                    if (string.Equals("software", constraintNode.Name.LocalName, StringComparison.Ordinal))
                    {
                        applications.Add(ExtractApplicationConstraint(constraintNode));
                    }
                }

                result.Add(new TestEnvironmentDescription(name, operatingSystem, applications));
            }

            return result;
        }

        private OperatingSystemDescription ExtractOperatingSystemConstraint(XElement input)
        {
            var name = input.Attribute("name").Value;
            var servicePack = input.Attribute("servicepack").Value;
            var culture = input.Attribute("culture").Value;
            var architecturePointerSize = int.Parse(input.Attribute("architecturepointersize").Value, CultureInfo.InvariantCulture);

            return new OperatingSystemDescription(name, servicePack, new CultureInfo(culture), architecturePointerSize);
        }

        private ApplicationDescription ExtractApplicationConstraint(XElement input)
        {
            var name = input.Attribute("name").Value;
            var version = new Version(input.Attribute("version").Value);

            return new ApplicationDescription(name, version);
        }

        private IEnumerable<TestStepDescription> ExtractTestSteps(XElement rootNode)
        {
            var testStepsNode = rootNode.Element("teststeps");

            var result = new List<TestStepDescription>();
            foreach (var testStepNode in testStepsNode.Elements())
            {
                TestStepDescription testStep;
                switch (testStepNode.Name.LocalName)
                {
                    case "msi":
                        testStep = ExtractMsiDeployTestStep(testStepNode);
                        break;
                    case "script":
                        testStep = ExtractScriptExecuteTestStep(testStepNode);
                        break;
                    case "xcopy":
                        testStep = ExtractXCopyDeployTestStep(testStepNode);
                        break;
                    default:
                        throw new UnknownTestStepConfigurationException();
                }

                result.Add(testStep);
            }

            return result;
        }

        /// <summary>
        /// Extracts the failure mode that describes what action should be taken in case of failure
        /// of a test step.
        /// </summary>
        /// <param name="node">The node that contains the failure mode attribute.</param>
        /// <returns>The failure mode, being either 'Stop' or 'Continue'.</returns>
        protected virtual string ExtractFailureModeFromTestStepConfiguration(XElement node)
        {
            return node.Attribute("onfailure").Value;
        }

        private MsiInstallTestStepDescription ExtractMsiDeployTestStep(XElement node)
        {
            var stepOrder = ExtractStepOrderFromTestStepConfiguration(node);
            var environment = ExtractEnvironmentNameFromTestStepConfiguration(node);
            var failureMode = ExtractFailureModeFromTestStepConfiguration(node);

            var file = (node.Element("file").FirstNode as XCData).Value;
            var parameters = ExtractParametersFromTestStepConfiguration(node);

            AddFileToEnvironmentPackage(environment, stepOrder, Path.GetFileName(file), file);

            return new MsiInstallTestStepDescription(environment, stepOrder, failureMode, parameters);
        }

        private ScriptExecuteTestStepDescription ExtractScriptExecuteTestStep(XElement node)
        {
            var stepOrder = ExtractStepOrderFromTestStepConfiguration(node);
            var environment = ExtractEnvironmentNameFromTestStepConfiguration(node);
            var failureMode = ExtractFailureModeFromTestStepConfiguration(node);

            var fileNode = node.Element("file");
            var language = fileNode.Attribute("language").Value;
            var file = (fileNode.FirstNode as XCData).Value;
            var parameters = ExtractParametersFromTestStepConfiguration(node);

            AddFileToEnvironmentPackage(environment, stepOrder, Path.GetFileName(file), file);

            return new ScriptExecuteTestStepDescription(environment, stepOrder, failureMode, parameters, language);
        }

        private XCopyTestStepDescription ExtractXCopyDeployTestStep(XElement node)
        {
            var stepOrder = ExtractStepOrderFromTestStepConfiguration(node);
            var environment = ExtractEnvironmentNameFromTestStepConfiguration(node);
            var failureMode = ExtractFailureModeFromTestStepConfiguration(node);

            var remoteBasePath = (node.Element("destination").FirstNode as XCData).Value;
            var basePath = (node.Element("base").FirstNode as XCData).Value;

            foreach (var element in node.Element("paths").Elements())
            {
                var path = (element.FirstNode as XCData).Value;
                if (string.Equals("file", element.Name.LocalName, StringComparison.Ordinal))
                {
                    var relativePath = path.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
                    AddFileToEnvironmentPackage(environment, stepOrder, relativePath, path);
                }

                if (string.Equals("directory", element.Name.LocalName, StringComparison.Ordinal))
                {
                    foreach (var file in m_FileSystem.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                    {
                        var relativePath = file.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
                        AddFileToEnvironmentPackage(environment, stepOrder, relativePath, file);
                    }
                }
            }

            return new XCopyTestStepDescription(environment, stepOrder, failureMode, remoteBasePath);
        }

        private string ExtractCompletedNotification(XElement rootNode)
        {
            var notificationNode = rootNode.Element("completednotification").Elements().FirstOrDefault();
            if (notificationNode == null)
            {
                throw new MissingNotificationConfigurationException();
            }

            var path = (notificationNode.Element("path").FirstNode as XCData).Value;
            return path;
        }
    }
}
