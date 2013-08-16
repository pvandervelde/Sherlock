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
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Moq;
using Nuclei;
using NUnit.Framework;

namespace Sherlock.Console
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ConfigurationReaderVersion10Test
    {
        private sealed class MockTestStepDescription : TestStepDescription
        {
            public MockTestStepDescription(string environment, int order, IEnumerable<TestStepParameterDescription> parameters) 
                : base(environment, order, parameters)
            {
            }
        }

        [Test]
        public void ReadWithIncorrectVersionNumber()
        {
            // Load the config file string
            var configText = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               "Sherlock.Console.Sherlock.Configuration.v1.xml");

            // Replace the version place holder
            configText = configText.Replace("${VERSION}$", new Version(2, 1).ToString());
            var doc = XDocument.Parse(configText, LoadOptions.None);

            var reader = new ConfigurationReaderVersion10(
                new List<IConstructTestSteps>());
            Assert.Throws<NonMatchingVersionFoundException>(() => reader.Read(doc));
        }

        [Test]
        public void Read()
        {
            const string configVersion = "1.0";
            const string serverUrl = @"http:\\www.myawesomesite\sherlock";
            const string product = "product";
            const string productVersion = "2.0";
            const string testPurpose = "client";

            const string operatingSystemName = "Os";
            const string operatingSystemServicePack = "SP300";
            const string operatingSystemCulture = "en-US";
            const string operatingSystemPointerSize = "32";

            const string applicationName = "application";
            const string applicationVersion = "1.0";

            const int stepOrder = 1;
            const int environmentIndex = 0;

            const string notificationPath = @"c:\a\b\c\d";

            // Load the config file string
            var text = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               "Sherlock.Console.Sherlock.Configuration.v1.xml");
            string configText;
            {
                var builder = new StringBuilder(text);
                builder.Replace("${VERSION}$", configVersion);
                builder.Replace("${SERVER_URL}$", serverUrl);

                builder.Replace("${NAME_OF_PRODUCT_UNDER_TEST}$", product);
                builder.Replace("${VERSION_OF_PRODUCT_UNDER_TEST}$", productVersion);
                builder.Replace("${PURPOSE_OF_TEST}$", testPurpose);

                builder.Replace("${OPERATING_SYSTEM_NAME}$", operatingSystemName);
                builder.Replace("${OPERATING_SYSTEM_SERVICE_PACK}$", operatingSystemServicePack);
                builder.Replace("${OPERATING_SYSTEM_CULTURE}$", operatingSystemCulture);
                builder.Replace("${OPERATING_SYSTEM_POINTER_SIZE}$", operatingSystemPointerSize);

                builder.Replace("${APPLICATION_NAME}$", applicationName);
                builder.Replace("${APPLICATION_VERSION}$", applicationVersion);

                builder.Replace("${STEP_ORDER_INDEX}$", stepOrder.ToString(CultureInfo.InvariantCulture));
                builder.Replace("${ENVIRONMENT_NAME}$", environmentIndex.ToString(CultureInfo.InvariantCulture));

                builder.Replace("${DIRECTORY_FULL_PATH_GOES_HERE}$", notificationPath);

                configText = builder.ToString();
            }

            var doc = XDocument.Parse(configText, LoadOptions.None);

            var testStep = new MockTestStepDescription("a", 0, new List<TestStepParameterDescription>());
            var testStepProcessor = new Mock<IConstructTestSteps>();
            {
                testStepProcessor.Setup(t => t.Contract)
                    .Returns("msi");
                testStepProcessor.Setup(t => t.Construct(It.IsAny<XElement>()))
                    .Returns(testStep);
            }

            var reader = new ConfigurationReaderVersion10(
                new List<IConstructTestSteps>
                    {
                        testStepProcessor.Object,
                    });
            var config = reader.Read(doc);
            Assert.IsNotNull(config);

            Assert.AreEqual(product, config.Test.ProductUnderTest);
            Assert.AreEqual(productVersion, config.Test.VersionOfProductUnderTest);
            Assert.AreEqual(testPurpose, config.Test.Description);

            Assert.AreEqual(1, config.Test.Environments.Count());

            Assert.AreEqual(1, config.Test.TestSteps.Count());

            Assert.That(
                config.Test.TestSteps, 
                Is.EquivalentTo(
                    new List<TestStepDescription>
                    {
                        testStep
                    }));
            Assert.AreEqual(notificationPath, config.Test.ReportPath);
        }
    }
}
