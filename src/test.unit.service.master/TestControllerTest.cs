//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using Autofac.Features.OwnedInstances;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;
using Test.Mocks;

namespace Sherlock.Service.Master
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TestControllerTest
    {
        private sealed class MockDisposable : IDisposable
        {
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // Do nothing
            }
        }

        private static Mock<IConfiguration> CreateConfiguration()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.Value<string>(It.Is<ConfigurationKey>(k => k.Equals(MasterServiceConfigurationKeys.TestDataDirectory))))
                    .Returns(@"a\b\c");
            }

            return configuration;
        }

        private static TestEnvironment CreateFakeTestEnvironment(string name)
        {
            var environment = new TestEnvironment
                {
                    Name = name,
                    OperatingSystem = new OperatingSystemDescription(),
                    TestApplications = new List<TestApplication>(),
                };

            return environment;
        }

        private static Shared.DataAccess.Test CreateFakeTestSpecification(TestEnvironment environment)
        {
            var test = new Shared.DataAccess.Test
                {
                    pk_TestId = 1,
                    ProductName = "tda",
                    ProductVersion = "tdb",
                    Owner = "tdc",
                    TestDescription = "tdd",
                    ReportPath = @"c:\d\e\f",
                    TestEnvironments = new List<TestEnvironment>
                        {
                            environment
                        }
                };
            
            return test;
        }

        private static Mock<IProvideTestingContext> CreateEnvironmentContext(
            PhysicalMachineDescription machineDescription, 
            Shared.DataAccess.Test test, 
            List<TestStep> steps)
        {
            var environmentContext = new Mock<IProvideTestingContext>();
            {
                environmentContext.Setup(
                    e => e.InactiveMachinesWith(It.IsAny<OperatingSystemDescription>(), It.IsAny<IEnumerable<ApplicationDescription>>()))
                    .Returns(
                        new List<MachineDescription>
                            {
                                machineDescription,
                            });
                environmentContext.Setup(e => e.InactiveTests())
                    .Returns(
                        new List<Shared.DataAccess.Test>
                            {
                                test
                            });
                environmentContext.Setup(e => e.TestStepsForEnvironment(It.IsAny<int>()))
                    .Returns(steps);
                environmentContext.Setup(e => e.StartTest(It.IsAny<int>()))
                    .Verifiable();
                environmentContext.Setup(e => e.MarkMachineAsActive(It.IsAny<string>()))
                    .Verifiable();
                environmentContext.Setup(e => e.TestEnvironmentSupportedByMachine(It.IsAny<int>(), It.IsAny<string>()))
                    .Verifiable();
            }

            return environmentContext;
        }

        [Test]
        public void ActivateTestsWithoutTests()
        {
            var configuration = CreateConfiguration();
            var activeTests = new ActiveTestStorage();

            var environmentPackage = new Mock<ITestEnvironmentPackage>();
            {
                environmentPackage.Setup(e => e.PackagePath)
                    .Returns("a");
            }

            var suitePackage = new Mock<ITestSuitePackage>();
            {
                suitePackage.Setup(s => s.Environment(It.IsAny<string>()))
                    .Returns(environmentPackage.Object);
            }

            Func<ITestSuitePackage> packageFunc = () => suitePackage.Object;

            var environmentContext = new Mock<IProvideTestingContext>();
            {
                environmentContext.Setup(
                    e => e.InactiveMachinesWith(
                        It.IsAny<OperatingSystemDescription>(),
                        It.IsAny<IEnumerable<ApplicationDescription>>()))
                    .Returns(
                        new List<MachineDescription>
                            {
                                new PhysicalMachineDescription
                                    {
                                        Id = "a-b",
                                        Name = "a",
                                        Description = "b",
                                        MacAddress = "c",
                                        NetworkName = "d",
                                        OperatingSystemId = 0,
                                        IsAvailableForTesting = true,
                                        CanStartRemotely = false,
                                    }
                            });
                environmentContext.Setup(e => e.InactiveTests())
                    .Returns(new List<Shared.DataAccess.Test>());
            }

            var activeEnvironment = new Mock<IActiveEnvironment>();
            var activator = new Mock<IEnvironmentActivator>();
            {
                activator.Setup(a => a.Load(It.IsAny<MachineDescription>(), It.IsAny<ITestSectionBuilder>(), It.IsAny<Action<string>>()))
                    .Returns(activeEnvironment.Object);
            }

            var activators = new List<IEnvironmentActivator>
                {
                    activator.Object,
                };
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            Func<IReportBuilder> reportBuilders = () => new Mock<IReportBuilder>().Object;
            Func<string, IReportBuilder, ITestSectionBuilder> sectionBuilders = (s, b) => new Mock<ITestSectionBuilder>().Object;
            var diagnostics = new SystemDiagnostics(
                (l, s) => { },
                null);

            var controller = new TestController(
                configuration.Object,
                activeTests,
                () => new Owned<IProvideTestingContext>(environmentContext.Object, new MockDisposable()),
                activators,
                packageFunc,
                fileSystem.Object,
                reportBuilders,
                sectionBuilders,
                diagnostics);
            controller.ActivateTests();
            Assert.AreEqual(0, activeTests.Count());
        }

        [Test]
        public void ActivateTestsWithoutEnvironments()
        {
            var configuration = CreateConfiguration();
            var environmentPackage = new Mock<ITestEnvironmentPackage>();
            {
                environmentPackage.Setup(e => e.PackagePath)
                    .Returns("a");
            }

            var suitePackage = new Mock<ITestSuitePackage>();
            {
                suitePackage.Setup(s => s.Environment(It.IsAny<string>()))
                    .Returns(environmentPackage.Object);
            }

            Func<ITestSuitePackage> packageFunc = () => suitePackage.Object;

            var activeTests = new ActiveTestStorage();
            var environmentContext = new Mock<IProvideTestingContext>();
            {
                environmentContext.Setup(
                    e => e.InactiveMachinesWith(
                        It.IsAny<OperatingSystemDescription>(),
                        It.IsAny<IEnumerable<ApplicationDescription>>()))
                    .Returns(new List<MachineDescription>());
                environmentContext.Setup(e => e.InactiveTests())
                    .Returns(
                        new List<Shared.DataAccess.Test>
                            {
                                new Shared.DataAccess.Test
                                    {
                                        pk_TestId = 10,
                                        IsReadyForExecution = true,
                                        Owner = "a",
                                        ProductName = "b",
                                        ProductVersion = "1.2.3.4",
                                        RequestTime = DateTimeOffset.Now,
                                        ReportPath = @"c:\d\e\f",
                                        TestEnvironments = new List<TestEnvironment>(),
                                    },
                            });
            }

            var activeEnvironment = new Mock<IActiveEnvironment>();
            var activator = new Mock<IEnvironmentActivator>();
            {
                activator.Setup(a => a.Load(It.IsAny<MachineDescription>(), It.IsAny<ITestSectionBuilder>(), It.IsAny<Action<string>>()))
                    .Returns(activeEnvironment.Object);
            }

            var activators = new List<IEnvironmentActivator>
                {
                    activator.Object,
                };
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            Func<IReportBuilder> reportBuilders = () => new Mock<IReportBuilder>().Object;
            Func<string, IReportBuilder, ITestSectionBuilder> sectionBuilders = (s, b) => new Mock<ITestSectionBuilder>().Object;
            var diagnostics = new SystemDiagnostics(
                (l, s) => { },
                null);

            var controller = new TestController(
                configuration.Object,
                activeTests,
                () => new Owned<IProvideTestingContext>(environmentContext.Object, new MockDisposable()),
                activators,
                packageFunc,
                fileSystem.Object,
                reportBuilders,
                sectionBuilders,
                diagnostics);
            controller.ActivateTests();
            Assert.AreEqual(0, activeTests.Count());
        }

        [Test]
        public void ActivateTestsWithSingleEnvironment()
        {
            var configuration = CreateConfiguration();
            var environmentName = "td";
            var testEnvironment = CreateFakeTestEnvironment(environmentName);
            var test = CreateFakeTestSpecification(testEnvironment);
            var steps = new List<TestStep> 
            { 
                new MsiInstallTestStep
                    {
                        Order = 0,
                        FailureMode = TestStepFailureMode.Continue,
                    } 
            };
            var activeTests = new ActiveTestStorage();
            var machineDescription = new PhysicalMachineDescription
                {
                    Id = "a-b",
                    Name = "ma",
                    Description = "mb",
                    MacAddress = "mc",
                    NetworkName = "md",
                    OperatingSystemId = 0,
                    IsAvailableForTesting = true,
                    CanStartRemotely = false,
                };
            var environmentContext = CreateEnvironmentContext(machineDescription, test, steps);

            var activeEnvironment = new Mock<IActiveEnvironment>();
            {
                activeEnvironment.Setup(
                        a => a.Execute(
                            It.IsAny<int>(), 
                            It.IsAny<IEnumerable<TestStep>>(),
                            It.IsAny<IEnumerable<InputParameter>>(),
                            It.IsAny<string>()))
                    .Callback<int, IEnumerable<TestStep>, IEnumerable<InputParameter>, string>(
                        (id, s, parameters, file) =>
                        {
                            Assert.AreEqual(test.Id, id);
                            Assert.AreEqual(1, s.Count());
                            Assert.IsInstanceOf(typeof(MsiInstallTestStep), s.First());
                            Assert.AreEqual(1, parameters.Count());

                            var parameter = parameters.First();
                            Assert.AreEqual(environmentName, parameter.Key);
                            Assert.AreEqual(machineDescription.NetworkName, parameter.Value);
                            Assert.False(string.IsNullOrWhiteSpace(file));
                        })
                    .Verifiable();
                activeEnvironment.Setup(a => a.Environment)
                    .Returns("ea");
            }

            var activator = new Mock<IEnvironmentActivator>();
            {
                activator.Setup(a => a.EnvironmentTypeToLoad)
                    .Returns(typeof(PhysicalMachineDescription));
                activator.Setup(a => a.Load(It.IsAny<MachineDescription>(), It.IsAny<ITestSectionBuilder>(), It.IsAny<Action<string>>()))
                    .Returns(activeEnvironment.Object);
            }

            var activators = new List<IEnvironmentActivator>
                {
                    activator.Object,
                };

            var environmentPackage = new Mock<ITestEnvironmentPackage>();
            {
                environmentPackage.Setup(e => e.PackagePath)
                    .Returns("a");
            }

            var suitePackage = new Mock<ITestSuitePackage>();
            {
                suitePackage.Setup(s => s.Environment(It.IsAny<string>()))
                    .Returns(environmentPackage.Object);
            }

            Func<ITestSuitePackage> packageFunc = () => suitePackage.Object;
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            Func<IReportBuilder> reportBuilders = () => new Mock<IReportBuilder>().Object;
            Func<string, IReportBuilder, ITestSectionBuilder> sectionBuilders = (s, b) => new Mock<ITestSectionBuilder>().Object;
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var controller = new TestController(
                configuration.Object,
                activeTests,
                () => new Owned<IProvideTestingContext>(environmentContext.Object, new MockDisposable()),
                activators,
                packageFunc,
                fileSystem.Object,
                reportBuilders,
                sectionBuilders,
                diagnostics);
            controller.ActivateTests();
            Assert.AreEqual(1, activeTests.Count());
            activeEnvironment.Verify(
                a => a.Execute(
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<TestStep>>(),
                    It.IsAny<IEnumerable<InputParameter>>(),
                    It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void ActivateTestsWithMultipleEnvironments()
        { 
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void CompleteTest()
        {
            // foobar();
        }
    }
}
