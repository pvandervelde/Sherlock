//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
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
    public sealed class ConsoleExecuteTestStepProcessorTest
    {
        [Test]
        public void ExecuteWithExceptionInConsoleRunner()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var runner = new Mock<IRunConsoleApplications>();
            {
                runner.Setup(r => r.Run(It.IsAny<string>(), It.IsAny<string[]>()))
                    .Throws<NotImplementedException>();
            }

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);

            var executor = new ConsoleExecuteTestStepProcessor(
                testFileLocation,
                diagnostics,
                runner.Object,
                fileSystem,
                sectionBuilder.Object);

            var data = new ConsoleExecuteTestStep
            {
                pk_TestStepId = 1,
                Order = 2,
                ExecutableFilePath = @"c:\c\o\n\sole.exe"
            };

            var result = executor.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Failed, result);
        }

        [Test]
        public void ExecuteWithNonZeroErrorCode()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var runner = new Mock<IRunConsoleApplications>();
            {
                runner.Setup(r => r.Run(It.IsAny<string>(), It.IsAny<string[]>()))
                    .Returns(1);
            }

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);

            var executor = new ConsoleExecuteTestStepProcessor(
                testFileLocation,
                diagnostics,
                runner.Object,
                fileSystem,
                sectionBuilder.Object);

            var data = new ConsoleExecuteTestStep
            {
                pk_TestStepId = 1,
                Order = 2,
                ExecutableFilePath = @"c:\c\o\n\sole.exe"
            };

            var result = executor.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Failed, result);
        }

        [Test]
        public void Execute()
        {
            RetrieveFileDataForTestStep testFileLocation = index => @"c:\a\b";

            var runner = new Mock<IRunConsoleApplications>();
            {
                runner.Setup(r => r.Run(It.IsAny<string>(), It.IsAny<string[]>()))
                    .Callback(() => runner.Raise(r => r.OnConsoleOutput += null, new ProcessOutputEventArgs("foo")))
                    .Returns(0);
            }

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var executor = new ConsoleExecuteTestStepProcessor(
                testFileLocation,
                diagnostics,
                runner.Object,
                fileSystem,
                sectionBuilder.Object);

            var parameters = new List<TestStepParameter>
                {
                    new TestStepParameter
                        {
                            Key = "0",
                            Value = "Value",
                        },
                };

            var data = new ConsoleExecuteTestStep
            {
                pk_TestStepId = 1,
                Order = 2,
                TestStepParameters = parameters,
                ExecutableFilePath = @"c:\c\o\n\sole.exe"
            };

            var result = executor.Process(data, new List<InputParameter>());
            Assert.AreEqual(TestExecutionState.Passed, result);
        }
    }
}
