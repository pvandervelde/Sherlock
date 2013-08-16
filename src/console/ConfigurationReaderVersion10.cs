//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// An <see cref="IVersionedConfigurationReader"/> that reads configuration files with version 1.0.
    /// </summary>
    internal sealed class ConfigurationReaderVersion10 : IVersionedConfigurationReader
    {
        /// <summary>
        /// Gets the version of the configuration that the current reader can read.
        /// </summary>
        public static Version VersionToRead
        {
            get
            {
                return new Version(1, 0);
            }
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
        private static bool CanReadConfigurationWithVersion(Version version)
        {
            var assemblyVersion = VersionToRead;
            return (version.Major == assemblyVersion.Major) && (version.Minor == assemblyVersion.Minor);
        }

        private static IEnumerable<TestEnvironmentDescription> ExtractEnvironmentConstraints(XElement rootNode)
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

        private static OperatingSystemDescription ExtractOperatingSystemConstraint(XElement input)
        {
            var name = input.Attribute("name").Value;
            var servicePack = input.Attribute("servicepack").Value;
            var culture = input.Attribute("culture").Value;
            var architecturePointerSize = int.Parse(input.Attribute("architecturepointersize").Value, CultureInfo.InvariantCulture);

            return new OperatingSystemDescription(name, servicePack, new CultureInfo(culture), architecturePointerSize);
        }

        private static ApplicationDescription ExtractApplicationConstraint(XElement input)
        {
            var name = input.Attribute("name").Value;
            var version = new Version(input.Attribute("version").Value);

            return new ApplicationDescription(name, version);
        }

        private static string ExtractCompletedNotification(XElement rootNode)
        {
            var notificationNode = rootNode.Element("completednotification").Elements().FirstOrDefault();
            if (notificationNode == null)
            {
                throw new MissingNotificationConfigurationException();
            }

            var path = (notificationNode.Element("path").FirstNode as XCData).Value;
            return path;
        }

        /// <summary>
        /// The collection of objects that can build test steps.
        /// </summary>
        private readonly IEnumerable<IConstructTestSteps> m_TestStepBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReaderVersion10"/> class.
        /// </summary>
        /// <param name="testStepBuilders">The collection of all known test step builders.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testStepBuilders"/> is <see langword="null" />.
        /// </exception>
        public ConfigurationReaderVersion10(
            IEnumerable<IConstructTestSteps> testStepBuilders)
        {
            {
                Lokad.Enforce.Argument(() => testStepBuilders);
            }

            m_TestStepBuilders = testStepBuilders;
        }

        /// <summary>
        /// Reads the configuration from the XML document.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>A new configuration information object.</returns>
        public ConfigurationInfo Read(XDocument xmlDocument)
        {
            // Check the version. It should match the current version of the assembly
            var rootNode = xmlDocument.Element("sherlock");

            var configurationVersion = new Version(rootNode.Attribute("configurationVersion").Value);
            if (!CanReadConfigurationWithVersion(configurationVersion))
            {
                throw new NonMatchingVersionFoundException(configurationVersion.ToString(), VersionToRead.ToString());
            }

            var serverNode = rootNode.Element("server");
            var serverUrl = serverNode.Element("url").Value;

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
            return new ConfigurationInfo(serverUrl, description);
        }

        private IEnumerable<TestStepDescription> ExtractTestSteps(XElement rootNode)
        {
            var testStepsNode = rootNode.Element("teststeps");

            var result = new List<TestStepDescription>();
            foreach (var testStepNode in testStepsNode.Elements())
            {
                var builder = m_TestStepBuilders.FirstOrDefault(
                    b => string.Equals(b.Contract, testStepNode.Name.LocalName, StringComparison.Ordinal));
                if (builder == null)
                {
                    throw new UnknownTestStepConfigurationException();
                }

                var testStep = builder.Construct(testStepNode);
                result.Add(testStep);
            }

            return result;
        }
    }
}
