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
using MbUnit.Framework;
using Moq;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Master
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class EnvironmentControllerTest
    {
        private sealed class MockEnvironmentSpecification : MachineEnvironmentSpecification
        {
            public MockEnvironmentSpecification(
                EnvironmentId id, 
                string contract, 
                string name, 
                string description, 
                bool isAvailableForTest, 
                bool shouldCleanAfterUse, 
                string networkName, 
                string macAddress, 
                OperatingSystemSpecification operatingSystem, 
                IEnumerable<ApplicationSpecification> installedApplications) 
                : base(
                    id, 
                    contract, 
                    name, 
                    description, 
                    isAvailableForTest, 
                    shouldCleanAfterUse, 
                    networkName, 
                    macAddress, 
                    operatingSystem, 
                    installedApplications)
            {
            }

            public override bool Equals(MachineEnvironmentSpecification other)
            {
                return Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override string ToString()
            {
                return "MockEnvironmentSpecification";
            }
        }

        [Test]
        public void ActivateWithMissingActivator()
        {
            var specification = new MockEnvironmentSpecification(
                new EnvironmentId("e"), 
                "a", 
                "b", 
                "c", 
                true, 
                true,
                "d",
                "e",
                new OperatingSystemSpecification("f", "g", CultureInfo.InvariantCulture, 64), 
                new List<ApplicationSpecification>());

            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var controller = new EnvironmentController(
                new List<MachineEnvironmentSpecification> 
                    {
                        specification,
                    },
                new List<IEnvironmentActivator>());

            Assert.Throws<ArgumentException>(() => controller.Activate(specification.Id, sectionBuilder.Object));
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void ActivateWithActiveEnvironment()
        { 
        }

        [Test]
        public void Activate()
        {
            var contract = "a";
            var specification = new MockEnvironmentSpecification(
                new EnvironmentId("e"),
                "a",
                "b",
                "c",
                true,
                true,
                "d",
                "e",
                new OperatingSystemSpecification("f", "g", CultureInfo.InvariantCulture, 64),
                new List<ApplicationSpecification>());
            var activeEnvironment = new Mock<IActiveEnvironment>();
            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var activator = new Mock<IEnvironmentActivator>();
            {
                activator.Setup(a => a.EnvironmentContractToLoad)
                    .Returns(contract);
                activator.Setup(
                        a => a.Load(
                            It.IsAny<MachineEnvironmentSpecification>(), 
                            It.IsAny<ITestSectionBuilder>(),
                            It.IsAny<Action<EnvironmentId>>()))
                    .Callback<MachineEnvironmentSpecification, ITestSectionBuilder, Action<EnvironmentId>>(
                        (e, t, a) => 
                        {
                            Assert.AreSame(specification, e);
                            Assert.IsNotNull(t);
                            Assert.IsNotNull(a);
                        })
                    .Returns(activeEnvironment.Object);
            }

            var controller = new EnvironmentController(
                new List<MachineEnvironmentSpecification> 
                    {
                        specification,
                    },
                new List<IEnvironmentActivator> 
                    {
                        activator.Object,
                    });

            var environment = controller.Activate(specification.Id, sectionBuilder.Object);
            Assert.AreSame(activeEnvironment.Object, environment);

            Assert.AreEqual(0, controller.CurrentlyInActive().Count());
            Assert.AreEqual(1, controller.CurrentlyActive().Count());
            Assert.AreSame(specification.Id, controller.CurrentlyActive().First());
        }

        [Test]
        public void ActivateAndDeactivate()
        {
            var contract = "a";
            var specification = new MockEnvironmentSpecification(
                new EnvironmentId("e"),
                "a",
                "b",
                "c",
                true,
                true,
                "d",
                "e",
                new OperatingSystemSpecification("f", "g", CultureInfo.InvariantCulture, 64),
                new List<ApplicationSpecification>());
            var activeEnvironment = new Mock<IActiveEnvironment>();

            Action<EnvironmentId> deactivate = null;
            var sectionBuilder = new Mock<ITestSectionBuilder>();
            var activator = new Mock<IEnvironmentActivator>();
            {
                activator.Setup(a => a.EnvironmentContractToLoad)
                    .Returns(contract);
                activator.Setup(
                        a => a.Load(
                            It.IsAny<MachineEnvironmentSpecification>(), 
                            It.IsAny<ITestSectionBuilder>(),
                            It.IsAny<Action<EnvironmentId>>()))
                    .Callback<MachineEnvironmentSpecification, ITestSectionBuilder, Action<EnvironmentId>>(
                        (e, t, a) =>
                        {
                            deactivate = a;
                        })
                    .Returns(activeEnvironment.Object);
            }

            var controller = new EnvironmentController(
                new List<MachineEnvironmentSpecification> 
                    {
                        specification,
                    },
                new List<IEnvironmentActivator> 
                    {
                        activator.Object,
                    });

            var environment = controller.Activate(specification.Id, sectionBuilder.Object);

            Assert.AreEqual(0, controller.CurrentlyInActive().Count());
            Assert.AreEqual(1, controller.CurrentlyActive().Count());
            Assert.AreSame(specification.Id, controller.CurrentlyActive().First());

            deactivate(specification.Id);

            Assert.AreEqual(1, controller.CurrentlyInActive().Count());
            Assert.AreSame(specification.Id, controller.CurrentlyInActive().First());
            Assert.AreEqual(0, controller.CurrentlyActive().Count());
        }
    }
}
