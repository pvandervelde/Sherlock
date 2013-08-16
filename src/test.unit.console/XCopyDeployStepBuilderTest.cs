//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Moq;
using Nuclei;
using NUnit.Framework;
using Test.Mocks;

namespace Sherlock.Console
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class XCopyDeployStepBuilderTest
    {
        [Test]
        [Description("Checks that an XML fragment is correctly parsed.")]
        public void ReadConfiguration()
        {
            // Configuration values:
            const string version = "1.0";
            const int stepIndex = 9;
            const string environmentIndex = "a";
            const string destination = @"c:\a\b\c";
            const string basePath = @"c:\d\e\f";
            const string installFile = @"c:\d\e\f\g\h.ijk";
            const string installDirectory = @"c:\d\e\f\g\l";

            var text = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                "Sherlock.Console.configuration.xcopy.xml");

            // Replace the version place holder
            string configText;
            {
                var builder = new StringBuilder(text);
                builder.Replace("${VERSION}$", version);
                builder.Replace("${STEP_ORDER_INDEX}$", stepIndex.ToString(CultureInfo.InvariantCulture));
                builder.Replace("${ENVIRONMENT_INDEX}$", environmentIndex.ToString(CultureInfo.InvariantCulture));
                builder.Replace("${DESTINATION_PATH}$", destination);
                builder.Replace("${BASE_PATH}$", basePath);
                builder.Replace("${FILE_FULL_PATH_GOES_HERE}$", installFile);
                builder.Replace("${DIRECTORY_FULL_PATH_GOES_HERE}$", installDirectory);

                configText = builder.ToString();
            }

            var element = XElement.Parse(configText, LoadOptions.None);

            var files = new Dictionary<string, string>
                {
                    {
                        @"c:\d\e\f\g\h.ijk", "h.ijk"
                    },
                    {
                        Path.Combine(installDirectory, "m.nop"), "m.nop"
                    },
                    {
                        Path.Combine(installDirectory, "q.rst"), "q.rst"
                    },
                };

            var file = new MockFile(files);
            var directory = new MockDirectory(
                new[]
                {
                    Path.Combine(installDirectory, "m.nop"),
                    Path.Combine(installDirectory, "q.rst")
                });
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(file);
                fileSystem.Setup(f => f.Directory)
                    .Returns(directory);
            }

            var relativeFiles = new List<string>
                {
                    @"g\h.ijk",
                    @"g\l\m.nop",
                    @"g\l\q.rst",
                };
            StoreFileDataForEnvironment storage =
                (environment, step, name, data) =>
                    {
                        Assert.AreEqual(environmentIndex, environment);
                        Assert.AreEqual(stepIndex, step);
                        Assert.IsTrue(relativeFiles.Contains(name));
                        Assert.IsTrue(files.ContainsKey(data));
                    };
            var stepBuilder = new XCopyDeployStepBuilder(fileSystem.Object, storage);
            var config = (XCopyTestStepDescription)stepBuilder.Construct(element);

            Assert.AreEqual(environmentIndex, config.Environment);
            Assert.AreEqual(stepIndex, config.Order);
            Assert.AreEqual(destination, config.Destination);
        }
    }
}
