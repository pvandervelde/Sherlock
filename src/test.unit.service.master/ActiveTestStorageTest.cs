//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Master
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ActiveTestStorageTest
    {
        [Test]
        public void Add()
        {
            var notifications = new List<TestCompletedNotification>
                {
                    new FileBasedTestCompletedNotification("b"),
                };

            var report = new Mock<IReport>();
            var builder = new Mock<IReportBuilder>();
            {
                builder.Setup(b => b.Build())
                    .Returns(report.Object);
            }

            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var storage = new ActiveTestStorage(diagnostics);

            var testId = 10;
            storage.Add(testId, builder.Object, notifications);

            Assert.That(
                storage.NotificationsFor(testId),
                Is.EquivalentTo(notifications));
            Assert.AreSame(report.Object, storage.ReportFor(testId));
        }

        [Test]
        public void AddEnvironmentForTest()
        {
            var notifications = new List<TestCompletedNotification>();
            var builder = new Mock<IReportBuilder>();
            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var storage = new ActiveTestStorage(diagnostics);

            var testId = 10;
            storage.Add(testId, builder.Object, notifications);

            var environment = new Mock<IActiveEnvironment>();
            {
                environment.Setup(e => e.Environment)
                    .Returns("a");
            }

            storage.AddEnvironmentForTest(testId, environment.Object);

            Assert.That(
                storage.EnvironmentsForTest(testId),
                Is.EquivalentTo(
                    new List<IActiveEnvironment>
                        {
                            environment.Object
                        }));
        }

        [Test]
        public void HandleEnvironmentExecutionProgress()
        {
            var notifications = new List<TestCompletedNotification>();
            var builder = new Mock<IReportBuilder>();
            {
                builder.Setup(b => b.AddToSection(It.IsAny<string>(), It.IsAny<IEnumerable<TestSection>>()))
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var storage = new ActiveTestStorage(diagnostics);

            var testId = 10;
            storage.Add(testId, builder.Object, notifications);

            var environment = new Mock<IActiveEnvironment>();
            {
                environment.Setup(e => e.Environment)
                    .Returns("a");
            }

            storage.AddEnvironmentForTest(testId, environment.Object);

            environment.Raise(
                e => e.OnExecutionProgress += null, 
                new TestExecutionProgressEventArgs(
                    testId, 
                    "a", 
                    new TestSection(
                        "b",
                        DateTimeOffset.Now,
                        DateTimeOffset.Now,
                        false,
                        Enumerable.Empty<DateBasedTestInformation>(),
                        Enumerable.Empty<DateBasedTestInformation>(),
                        Enumerable.Empty<DateBasedTestInformation>())));
            builder.Verify(b => b.AddToSection(It.IsAny<string>(), It.IsAny<IEnumerable<TestSection>>()), Times.Once());
        }

        [Test]
        public void HandleEnvironmentTestCompletion()
        {
            var notifications = new List<TestCompletedNotification>();
            var builder = new Mock<IReportBuilder>();
            {
                builder.Setup(b => b.AddToSection(It.IsAny<string>(), It.IsAny<IEnumerable<TestSection>>()))
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var storage = new ActiveTestStorage(diagnostics);

            var testCompleted = false;
            storage.OnTestCompletion += (s, e) => testCompleted = true;

            var testId = 10;
            storage.Add(testId, builder.Object, notifications);

            var environment = new Mock<IActiveEnvironment>();
            {
                environment.Setup(e => e.Environment)
                    .Returns("a");
            }

            storage.AddEnvironmentForTest(testId, environment.Object);

            environment.Raise(e => e.OnTestCompletion += null, new TestExecutionResultEventArgs(testId, TestExecutionResult.Failed));
            Assert.IsTrue(testCompleted);
        }

        [Test]
        public void Remove()
        {
            var notifications = new List<TestCompletedNotification>();
            var builder = new Mock<IReportBuilder>();
            {
                builder.Setup(b => b.AddToSection(It.IsAny<string>(), It.IsAny<IEnumerable<TestSection>>()))
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var storage = new ActiveTestStorage(diagnostics);

            var testId = 10;
            storage.Add(testId, builder.Object, notifications);
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new List<int>
                        {
                            testId
                        }));

            storage.Remove(testId);
            Assert.AreEqual(0, storage.Count());
        }

        [Test]
        public void EnvironmentFailure()
        {
            var notifications = new List<TestCompletedNotification>();
            var builder = new Mock<IReportBuilder>();
            {
                builder.Setup(b => b.AddToSection(It.IsAny<string>(), It.IsAny<IEnumerable<TestSection>>()))
                    .Verifiable();
            }

            var diagnostics = new SystemDiagnostics((p, s) => { }, null);
            var storage = new ActiveTestStorage(diagnostics);

            var testCompleted = false;
            var testResult = TestExecutionResult.None;
            storage.OnTestCompletion += 
                (s, e) =>
                {
                    testCompleted = true;
                    testResult = e.Result;
                };

            var testId = 10;
            storage.Add(testId, builder.Object, notifications);

            var environmentId = "a";
            var firstEnvironment = new Mock<IActiveEnvironment>();
            {
                firstEnvironment.Setup(e => e.Environment)
                    .Returns(environmentId);
                firstEnvironment.Setup(e => e.Terminate())
                    .Verifiable();
            }

            var secondEnvironment = new Mock<IActiveEnvironment>();
            {
                secondEnvironment.Setup(e => e.Environment)
                    .Returns(environmentId);
                secondEnvironment.Setup(e => e.Terminate())
                    .Verifiable();
            }

            storage.AddEnvironmentForTest(testId, firstEnvironment.Object);
            storage.AddEnvironmentForTest(testId, secondEnvironment.Object);

            storage.EnvironmentFailure(testId, environmentId);

            Assert.IsTrue(testCompleted);
            Assert.AreEqual(TestExecutionResult.Failed, testResult);
            firstEnvironment.Verify(e => e.Terminate(), Times.Once());
            secondEnvironment.Verify(e => e.Terminate(), Times.Once());
        }
    }
}
