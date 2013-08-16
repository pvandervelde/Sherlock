//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
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
    public sealed class ScriptExecuteStepBuilderTest
    {
        [Test]
        [Description("Checks that an XML fragment is correctly parsed.")]
        public void ReadConfiguration()
        {
            // Configuration values:
            const string version = "1.0";
            const int stepIndex = 9;
            const string environmentIndex = "a";
            const string scriptLanguage = "Powershell";
            const string path = @"c:\a\b\c.def";
            const string parameterKey = "Parameter_Key";
            const string parameterValue = "Parameter_Value";
            const string content = "test";

            var text = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                "Sherlock.Console.configuration.script.xml");

            // Replace the version place holder
            string configText;
            {
                var builder = new StringBuilder(text);
                builder.Replace("${VERSION}$", version);
                builder.Replace("${STEP_ORDER_INDEX}$", stepIndex.ToString(CultureInfo.InvariantCulture));
                builder.Replace("${ENVIRONMENT_INDEX}$", environmentIndex.ToString(CultureInfo.InvariantCulture));
                builder.Replace("${SCRIPT_LANGUAGE_GOES_HERE}$", scriptLanguage.ToString());
                builder.Replace("${SCRIPT_FULL_PATH_GOES_HERE}$", path);
                builder.Replace("${PARAMETER_KEY}$", parameterKey);
                builder.Replace("${PARAMETER_VALUE}$", parameterValue);

                configText = builder.ToString();
            }

            var element = XElement.Parse(configText, LoadOptions.None);

            var file = new MockFile(path, content);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(file);
            }

            StoreFileDataForEnvironment storage =
                (environment, step, name, data) =>
                    {
                        Assert.AreEqual(environmentIndex, environment);
                        Assert.AreEqual(stepIndex, step);
                        Assert.AreEqual(Path.GetFileName(path), name);
                        Assert.AreEqual(path, data);
                    };
            var stepBuilder = new ScriptExecuteStepBuilder(fileSystem.Object, storage);
            var config = (ScriptExecuteTestStepDescription)stepBuilder.Construct(element);

            Assert.AreEqual(environmentIndex, config.Environment);
            Assert.AreEqual(stepIndex, config.Order);
            Assert.AreEqual(scriptLanguage, config.ScriptLanguage);

            Assert.AreEqual(1, config.Parameters.Count());
            Assert.AreEqual(parameterKey, config.Parameters.First().Key);
            Assert.AreEqual(parameterValue, config.Parameters.First().Value);
        }
    }
}
