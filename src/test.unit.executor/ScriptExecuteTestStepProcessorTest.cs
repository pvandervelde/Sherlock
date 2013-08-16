//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptExecuteTestStepProcessorTest
    {
        [Test]
        public void InstallWithFailingPowershellFile()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                    {
                        { @"c:\a\b\c.ps1", new MockFileData("throw 'FAIL'") }
                    });

            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var installer = new ScriptExecuteTestStepProcessor(
               testFileLocation,
               diagnostics,
               fileSystem,
               sectionBuilder.Object);

            var data = new ScriptExecuteTestStep
                {
                    pk_TestStepId = 1,
                    Order = 2,
                    ScriptLanguage = ScriptLanguage.Powershell,
                };

            var result = installer.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Crashed, result);
        }

        [Test]
        [Description("Checks that a Powershell script can be run.")]
        public void InstallWithPowershellFile()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                    {
                        { @"c:\a\b\c.ps1", new MockFileData("Out-Host -InputObject 'hello word'") }
                    });

            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var installer = new ScriptExecuteTestStepProcessor(
               testFileLocation,
               diagnostics,
               fileSystem,
               sectionBuilder.Object);

            var data = new ScriptExecuteTestStep
            {
                pk_TestStepId = 1,
                Order = 2,
                ScriptLanguage = ScriptLanguage.Powershell,
            };

            var result = installer.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Passed, result);
        }
    }
}
