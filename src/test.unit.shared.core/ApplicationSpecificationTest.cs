//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Sherlock.Shared.Core
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationSpecificationTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ApplicationSpecification>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: 
            // http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                  new List<string> 
                     {
                        // We could generate a stack of strings here .. For now we'll stick with
                        // about 5 strings.
                        "a",
                        "b",
                        "c",
                        "d",
                        "e",
                     },
                   new List<Version> 
                     {
                        new Version(1, 2, 3, 4),
                        new Version(2, 2, 3, 4),
                        new Version(1, 3, 3, 4),
                        new Version(1, 2, 4, 4),
                        new Version(1, 2, 3, 5),
                     })
               .Select(o => new ApplicationSpecification(o.First, o.Second)),
        };

        [VerifyContract]
        public readonly IContract ComparableVerification = new ComparisonContract<ApplicationSpecification>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
               { 
                  new ApplicationSpecification("a", new Version(1, 2, 3, 4)),
                  new ApplicationSpecification("a", new Version(2, 2, 3, 4)),
                  new ApplicationSpecification("b", new Version(1, 2, 3, 4)),
               },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ApplicationSpecification>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection 
               { 
                  new ApplicationSpecification("a", new Version(1, 2, 3, 4)),
                  new ApplicationSpecification("a", new Version(2, 2, 3, 4)),
                  new ApplicationSpecification("b", new Version(1, 2, 3, 4)),
               },
        };

        [Test]
        public void Create()
        {
            var name = "a";
            var version = new Version(10, 9, 8, 7);
            var spec = new ApplicationSpecification(name, version);

            Assert.AreSame(name, spec.ApplicationName);
            Assert.AreSame(version, spec.ApplicationVersion);
        }

        [Test]
        public void IsNewerVersionOfWithNullOther()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsFalse(config1.IsNewerVersionOf(null));
        }

        [Test]
        public void IsNewerVersionOfWithSelf()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsFalse(config1.IsNewerVersionOf(config1));
        }

        [Test]
        public void IsNewerVersionOfWithUnequalAppName()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            var config2 = new ApplicationSpecification("app2", new Version(1, 0));

            Assert.IsFalse(config1.IsNewerVersionOf(config2));
        }

        [Test]
        public void IsNewerVersionOfWithOlderVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(2, 0));

            Assert.IsFalse(config1.IsNewerVersionOf(config2));
        }

        [Test]
        public void IsNewerVersionOfWithEqualVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsFalse(config1.IsNewerVersionOf(config2));
        }

        [Test]
        public void IsNewerVersionOfWithNewerVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 1));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsTrue(config1.IsNewerVersionOf(config2));
        }

        [Test]
        public void IsNewerOrEqualVersionOfWithNullOther()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsFalse(config1.IsNewerOrEqualVersionOf(null));
        }

        [Test]
        public void IsNewerOrEqualVersionOfWithSelf()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsTrue(config1.IsNewerOrEqualVersionOf(config1));
        }

        [Test]
        public void IsNewerOrEqualVersionOfWithUnequalAppName()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            var config2 = new ApplicationSpecification("app2", new Version(1, 0));

            Assert.IsFalse(config1.IsNewerOrEqualVersionOf(config2));
        }

        [Test]
        public void IsNewerOrEqualVersionOfWithOlderVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(2, 0));

            Assert.IsFalse(config1.IsNewerOrEqualVersionOf(config2));
        }

        [Test]
        public void IsNewerOrEqualVersionOfWithEqualVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsTrue(config1.IsNewerOrEqualVersionOf(config2));
        }

        [Test]
        public void IsNewerOrEqualVersionOfWithNewerVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 1));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsTrue(config1.IsNewerOrEqualVersionOf(config2));
        }

        [Test]
        public void IsOlderVersionOfWithNullOther()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsFalse(config1.IsOlderVersionOf(null));
        }

        [Test]
        public void IsOlderVersionOfWithSelf()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsFalse(config1.IsOlderVersionOf(config1));
        }

        [Test]
        public void IsOlderVersionOfWithUnequalAppName()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            var config2 = new ApplicationSpecification("app2", new Version(1, 0));

            Assert.IsFalse(config1.IsOlderVersionOf(config2));
        }

        [Test]
        public void IsOlderVersionOfWithOlderVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(2, 0));

            Assert.IsTrue(config1.IsOlderVersionOf(config2));
        }

        [Test]
        public void IsOlderVersionOfWithEqualVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsFalse(config1.IsOlderVersionOf(config2));
        }

        [Test]
        public void IsOlderVersionOfWithNewerVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 1));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsFalse(config1.IsOlderVersionOf(config2));
        }

        [Test]
        public void IsOlderOrEqualVersionOfWithNullOther()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsFalse(config1.IsOlderOrEqualVersionOf(null));
        }

        [Test]
        public void IsOlderOrEqualVersionOfWithSelf()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            Assert.IsTrue(config1.IsOlderOrEqualVersionOf(config1));
        }

        [Test]
        public void IsOlderOrEqualVersionOfWithUnequalAppName()
        {
            var config1 = new ApplicationSpecification("app1", new Version(1, 0));
            var config2 = new ApplicationSpecification("app2", new Version(1, 0));

            Assert.IsFalse(config1.IsOlderOrEqualVersionOf(config2));
        }

        [Test]
        public void IsOlderOrEqualVersionOfWithOlderVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(2, 0));

            Assert.IsTrue(config1.IsOlderOrEqualVersionOf(config2));
        }

        [Test]
        public void IsOlderOrEqualVersionOfWithEqualVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 0));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsTrue(config1.IsOlderOrEqualVersionOf(config2));
        }

        [Test]
        public void IsOlderOrEqualVersionOfWithNewerVersion()
        {
            var config1 = new ApplicationSpecification("app", new Version(1, 1));
            var config2 = new ApplicationSpecification("app", new Version(1, 0));

            Assert.IsFalse(config1.IsOlderOrEqualVersionOf(config2));
        }
    }
}
