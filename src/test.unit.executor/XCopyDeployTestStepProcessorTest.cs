//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;
using MockDirectory = Test.Mocks.MockDirectory;
using MockFile = Test.Mocks.MockFile;

namespace Sherlock.Executor
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class XCopyDeployTestStepProcessorTest
    {
        [Test]
        public void CreateWithEmptyInstallDirectory()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(
                new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                    {
                        { @"c:\d\e\f.ps1", new System.IO.Abstractions.TestingHelpers.MockFileData("throw 'FAIL'") }
                    });

            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var installer = new XCopyDeployTestStepProcessor(
               testFileLocation,
               diagnostics,
               fileSystem,
               sectionBuilder.Object);

            var data = new XCopyTestStep
                {
                    pk_TestStepId = 1,
                    Order = 2,
                    Destination = @"c:\d\e",
                };

            var result = installer.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Passed, result);
        }

        [Test]
        public void InstallWithNonExistingDestination()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem(
                new Dictionary<string, System.IO.Abstractions.TestingHelpers.MockFileData>
                    {
                        { @"c:\a\b\c.ps1", new System.IO.Abstractions.TestingHelpers.MockFileData("throw 'FAIL'") }
                    });

            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var installer = new XCopyDeployTestStepProcessor(
               testFileLocation,
               diagnostics,
               fileSystem,
               sectionBuilder.Object);

            var data = new XCopyTestStep
            {
                pk_TestStepId = 1,
                Order = 2,
                Destination = @"c:\d\e",
            };

            var result = installer.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Passed, result);
        }

        [Test]
        public void Install()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var files = new Dictionary<string, string>
                {
                    {
                        @"c:\a\b\c.ps1", "throw 'FAIL'"
                    },
                    {
                        @"c:\a\b\d\e.ps1", "throw 'FAIL'"
                    },
                    {
                        @"c:\a\b\d\f.ps1", "throw 'FAIL'"
                    },
                    {
                        @"c:\a\b\d\g.ps1", "throw 'FAIL'"
                    },
                };

            var file = new MockFile(files);
            var directory = new MockDirectory(
                new[]
                {
                    @"c:\a\b\d\e.ps1",
                    @"c:\a\b\d\f.ps1",
                    @"c:\a\b\d\g.ps1",
                });
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(file);
                fileSystem.Setup(f => f.Directory)
                    .Returns(directory);
            }

            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var installer = new XCopyDeployTestStepProcessor(
               testFileLocation,
               diagnostics,
               fileSystem.Object,
               sectionBuilder.Object);

            var data = new XCopyTestStep
            {
                pk_TestStepId = 1,
                Order = 2,
                Destination = @"c:\h\i",
            };

            var result = installer.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Passed, result);
        }
    }
}
