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
    public sealed class ConfigurationReaderVersion11Test
    {
        private delegate void BuildStep(StringBuilder builder);

        private delegate void FileStep(string environment, int step, string name, string data);

        private static Tuple<BuildStep, FileStep, Action<ConsoleExecuteTestStepDescription>> BuildConsoleExecuteTestStepFunctions(
            int index,
            string environmentName,
            Dictionary<string, string> filePathAndContent)
        {
            const string failureAction = "Console_Failure_Action";
            const string path = @"c:\c\o\nsole.exe";
            const string parameterValue = "Parameter_Value";

            BuildStep buildAction =
                builder =>
                {
                    builder.Replace("${CONSOLE_STEP_ORDER_INDEX}$", index.ToString(CultureInfo.InvariantCulture));
                    builder.Replace("${CONSOLE_ENVIRONMENT_NAME}$", environmentName);
                    builder.Replace("${CONSOLE_FAILURE_ACTION}$", failureAction);
                    builder.Replace("${CONSOLE_EXECUTABLE_FULL_PATH_GOES_HERE}$", path);
                    builder.Replace("${CONSOLE_PARAMETER_VALUE}$", parameterValue);
                };

            filePathAndContent.Add(path, "consoleContent");

            FileStep fileAction =
                (environment, step, name, data) =>
                {
                    Assert.AreEqual(environmentName, environment);
                    Assert.AreEqual(index, step);
                    Assert.AreEqual(Path.GetFileName(path), name);
                    Assert.AreEqual(path, data);
                };

            Action<ConsoleExecuteTestStepDescription> verifyAction =
                description =>
                {
                    Assert.AreEqual(environmentName, description.Environment);
                    Assert.AreEqual(index, description.Order);
                    Assert.AreEqual(failureAction, description.FailureMode);
                    Assert.AreEqual(path, description.ExecutablePath);

                    Assert.AreEqual(1, description.Parameters.Count());
                    Assert.AreEqual(0.ToString(CultureInfo.InvariantCulture), description.Parameters.First().Key);
                    Assert.AreEqual(parameterValue, description.Parameters.First().Value);

                    Assert.IsFalse(description.IncludeSystemLogFileInReport);
                    Assert.IsFalse(description.FileElementsToIncludeInReport.Any());
                };

            return new Tuple<BuildStep, FileStep, Action<ConsoleExecuteTestStepDescription>>(buildAction, fileAction, verifyAction);
        }

        private static Tuple<BuildStep, FileStep, Action<MsiInstallTestStepDescription>> BuildMsiDeployTestStepFunctions(
            int index,
            string environmentName,
            Dictionary<string, string> filePathAndContent)
        {
            const string failureAction = "Msi_Failure_Action";
            const string path = @"c:\m\s\i.msi";
            const string parameterKey = "Parameter_Key";
            const string parameterValue = "Parameter_Value";

            BuildStep buildAction =
                builder =>
                {
                    builder.Replace("${MSI_STEP_ORDER_INDEX}$", index.ToString(CultureInfo.InvariantCulture));
                    builder.Replace("${MSI_ENVIRONMENT_NAME}$", environmentName);
                    builder.Replace("${MSI_FAILURE_ACTION}$", failureAction);
                    builder.Replace("${MSI_INSTALLER_FULL_PATH_GOES_HERE}$", path);
                    builder.Replace("${MSI_PARAMETER_KEY}$", parameterKey);
                    builder.Replace("${MSI_PARAMETER_VALUE}$", parameterValue);
                };

            filePathAndContent.Add(path, "msiContent");

            FileStep fileAction =
                (environment, step, name, data) =>
                {
                    Assert.AreEqual(environmentName, environment);
                    Assert.AreEqual(index, step);
                    Assert.AreEqual(Path.GetFileName(path), name);
                    Assert.AreEqual(path, data);
                };

            Action<MsiInstallTestStepDescription> verifyAction =
                description =>
                {
                    Assert.AreEqual(environmentName, description.Environment);
                    Assert.AreEqual(index, description.Order);
                    Assert.AreEqual(failureAction, description.FailureMode);

                    Assert.AreEqual(1, description.Parameters.Count());
                    Assert.AreEqual(parameterKey, description.Parameters.First().Key);
                    Assert.AreEqual(parameterValue, description.Parameters.First().Value);

                    Assert.IsFalse(description.IncludeSystemLogFileInReport);
                    Assert.IsFalse(description.FileElementsToIncludeInReport.Any());
                };

            return new Tuple<BuildStep, FileStep, Action<MsiInstallTestStepDescription>>(buildAction, fileAction, verifyAction);
        }

        private static Tuple<BuildStep, FileStep, Action<ScriptExecuteTestStepDescription>> BuildScriptExecuteTestStepFunctions(
            int index,
            string environmentName,
            Dictionary<string, string> filePathAndContent)
        {
            const string failureAction = "Script_Failure_Action";
            const string scriptLanguage = "Powershell";
            const string scriptPath = @"c:\s\c\ript.ps1";
            const string scriptParameterKey = "Script_Parameter_Key";
            const string scriptParameterValue = "Script_Parameter_Value";

            BuildStep buildAction =
                builder =>
                {
                    builder.Replace("${SCRIPT_STEP_ORDER_INDEX}$", index.ToString(CultureInfo.InvariantCulture));
                    builder.Replace("${SCRIPT_ENVIRONMENT_NAME}$", environmentName.ToString(CultureInfo.InvariantCulture));
                    builder.Replace("${SCRIPT_FAILURE_ACTION}$", failureAction);
                    builder.Replace("${SCRIPT_LANGUAGE_GOES_HERE}$", scriptLanguage);
                    builder.Replace("${SCRIPT_FULL_PATH_GOES_HERE}$", scriptPath);
                    builder.Replace("${SCRIPT_PARAMETER_KEY}$", scriptParameterKey);
                    builder.Replace("${SCRIPT_PARAMETER_VALUE}$", scriptParameterValue);
                };

            filePathAndContent.Add(scriptPath, "scriptContent");

            FileStep fileAction =
                (environment, step, name, data) =>
                {
                    Assert.AreEqual(environmentName, environment);
                    Assert.AreEqual(index, step);
                    Assert.AreEqual(Path.GetFileName(scriptPath), name);
                    Assert.AreEqual(scriptPath, data);
                };

            Action<ScriptExecuteTestStepDescription> verifyAction =
                description =>
                {
                    Assert.AreEqual(environmentName, description.Environment);
                    Assert.AreEqual(index, description.Order);
                    Assert.AreEqual(failureAction, description.FailureMode);
                    Assert.AreEqual(scriptLanguage, description.ScriptLanguage);

                    Assert.AreEqual(1, description.Parameters.Count());
                    Assert.AreEqual(scriptParameterKey, description.Parameters.First().Key);
                    Assert.AreEqual(scriptParameterValue, description.Parameters.First().Value);

                    Assert.IsFalse(description.IncludeSystemLogFileInReport);
                    Assert.IsFalse(description.FileElementsToIncludeInReport.Any());
                };

            return new Tuple<BuildStep, FileStep, Action<ScriptExecuteTestStepDescription>>(buildAction, fileAction, verifyAction);
        }

        private static Tuple<BuildStep, FileStep, Action<XCopyTestStepDescription>, string[]> BuildXCopyDeployTestStepFunctions(
            int index,
            string environmentName,
            Dictionary<string, string> filePathAndContent)
        {
            const string failureAction = "XCopy_Failure_Action";
            const string destination = @"c:\a\b\c";
            const string basePath = @"c:\d\e\f";
            const string installFile = @"c:\d\e\f\g\h.ijk";
            const string installDirectory = @"c:\d\e\f\g\l";

            BuildStep buildAction =
                builder =>
                {
                    builder.Replace("${XCOPY_STEP_ORDER_INDEX}$", index.ToString(CultureInfo.InvariantCulture));
                    builder.Replace("${XCOPY_ENVIRONMENT_NAME}$", environmentName.ToString(CultureInfo.InvariantCulture));
                    builder.Replace("${XCOPY_FAILURE_ACTION}$", failureAction);
                    builder.Replace("${XCOPY_DESTINATION_PATH}$", destination);
                    builder.Replace("${XCOPY_BASE_PATH}$", basePath);
                    builder.Replace("${XCOPY_FILE_FULL_PATH_GOES_HERE}$", installFile);
                    builder.Replace("${XCOPY_DIRECTORY_FULL_PATH_GOES_HERE}$", installDirectory);
                };

            filePathAndContent.Add(installFile, "h.ijk");
            filePathAndContent.Add(Path.Combine(installDirectory, "m.nop"), "m.nop");
            filePathAndContent.Add(Path.Combine(installDirectory, "q.rst"), "q.rst");

            var relativeFiles = new List<string>
                {
                    @"g\h.ijk",
                    @"g\l\m.nop",
                    @"g\l\q.rst",
                };
            FileStep fileAction =
                (environment, step, name, data) =>
                {
                    Assert.AreEqual(environmentName, environment);
                    Assert.AreEqual(index, step);
                    Assert.IsTrue(relativeFiles.Contains(name));
                    Assert.IsTrue(filePathAndContent.ContainsKey(data));
                };

            Action<XCopyTestStepDescription> verifyAction =
                description =>
                {
                    Assert.AreEqual(environmentName, description.Environment);
                    Assert.AreEqual(index, description.Order);
                    Assert.AreEqual(failureAction, description.FailureMode);
                    Assert.AreEqual(destination, description.Destination);

                    Assert.IsFalse(description.IncludeSystemLogFileInReport);
                    Assert.IsFalse(description.FileElementsToIncludeInReport.Any());
                };

            return new Tuple<BuildStep, FileStep, Action<XCopyTestStepDescription>, string[]>(
                buildAction,
                fileAction,
                verifyAction,
                new[]
                {
                    Path.Combine(installDirectory, "m.nop"),
                    Path.Combine(installDirectory, "q.rst")
                });
        }

        [Test]
        public void ReadWithIncorrectVersionNumber()
        {
            // Load the config file string
            var configText = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               "Sherlock.Console.Sherlock.Configuration.v12.xml");

            // Replace the version place holder
            configText = configText.Replace("${VERSION}$", new Version(2, 1).ToString());
            var doc = XDocument.Parse(configText, LoadOptions.None);

            var fileSystem = new Mock<IFileSystem>();
            StoreFileDataForEnvironment storage = (environment, step, name, data) => { };

            var reader = new ConfigurationReaderVersion11(fileSystem.Object, storage);
            Assert.Throws<NonMatchingVersionFoundException>(() => reader.Read(doc));
        }

        [Test]
        public void Read()
        {
            const string product = "product";
            const string productVersion = "2.0";
            const string testPurpose = "client";

            const string operatingSystemName = "Os";
            const string operatingSystemServicePack = "SP300";
            const string operatingSystemCulture = "en-US";
            const string operatingSystemPointerSize = "32";

            const string applicationName = "application";
            const string applicationVersion = "1.0";

            const string environmentName = "environment_a";
            const string notificationPath = @"c:\a\b\c\d";

            // Note that the order of the elements is purposefully out of document order to 
            // test the ordering of the test steps
            var files = new Dictionary<string, string>();
            var console = BuildConsoleExecuteTestStepFunctions(3, environmentName, files);
            var msi = BuildMsiDeployTestStepFunctions(1, environmentName, files);
            var script = BuildScriptExecuteTestStepFunctions(2, environmentName, files);
            var xcopy = BuildXCopyDeployTestStepFunctions(0, environmentName, files);

            // Load the config file string
            var text = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
               Assembly.GetExecutingAssembly(),
               "Sherlock.Console.Sherlock.Configuration.v11.xml");
            string configText;
            {
                var builder = new StringBuilder(text);
                builder.Replace("${NAME_OF_PRODUCT_UNDER_TEST}$", product);
                builder.Replace("${VERSION_OF_PRODUCT_UNDER_TEST}$", productVersion);
                builder.Replace("${PURPOSE_OF_TEST}$", testPurpose);

                builder.Replace("${OPERATING_SYSTEM_NAME}$", operatingSystemName);
                builder.Replace("${OPERATING_SYSTEM_SERVICE_PACK}$", operatingSystemServicePack);
                builder.Replace("${OPERATING_SYSTEM_CULTURE}$", operatingSystemCulture);
                builder.Replace("${OPERATING_SYSTEM_POINTER_SIZE}$", operatingSystemPointerSize);

                builder.Replace("${APPLICATION_NAME}$", applicationName);
                builder.Replace("${APPLICATION_VERSION}$", applicationVersion);

                console.Item1(builder);
                msi.Item1(builder);
                script.Item1(builder);
                xcopy.Item1(builder);

                builder.Replace("${NOTIFICATION_DIRECTORY_FULL_PATH_GOES_HERE}$", notificationPath);

                configText = builder.ToString();
            }

            var doc = XDocument.Parse(configText, LoadOptions.None);

            var file = new MockFile(files);
            var directory = new MockDirectory(xcopy.Item4);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(file);
                fileSystem.Setup(f => f.Directory)
                    .Returns(directory);
            }

            StoreFileDataForEnvironment storage =
                (environment, step, name, data) =>
                {
                    switch (step)
                    {
                        case 0:
                            xcopy.Item2(environment, step, name, data);
                            break;
                        case 1:
                            msi.Item2(environment, step, name, data);
                            break;
                        case 2:
                            script.Item2(environment, step, name, data);
                            break;
                        case 3:
                            console.Item2(environment, step, name, data);
                            break;
                        default:
                            Assert.Fail();
                            break;
                    }
                };

            var reader = new ConfigurationReaderVersion11(fileSystem.Object, storage);
            var config = reader.Read(doc);
            Assert.IsNotNull(config);

            Assert.AreEqual(product, config.Test.ProductUnderTest);
            Assert.AreEqual(productVersion, config.Test.VersionOfProductUnderTest);
            Assert.AreEqual(testPurpose, config.Test.Description);

            Assert.AreEqual(1, config.Test.Environments.Count());

            var testSteps = config.Test.TestSteps.ToList();
            console.Item3(testSteps[0] as ConsoleExecuteTestStepDescription);
            msi.Item3(testSteps[1] as MsiInstallTestStepDescription);
            script.Item3(testSteps[2] as ScriptExecuteTestStepDescription);
            xcopy.Item3(testSteps[3] as XCopyTestStepDescription);

            Assert.AreEqual(notificationPath, config.Test.ReportPath);
        }
    }
}
