//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
   [TestFixture]
   [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
   public sealed class TestSectionTest
   {
      [Test]
      public void RoundTripSerialise()
      {
         var name = "someName";
         var start = DateTimeOffset.Now;
         var end = start.AddSeconds(10);
         bool success = false;
         var info = new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(1), "info") };
         var warning = new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(2), "warning") };
         var error = new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(3), "error") };

         var section = new TestSection(name, start, end, success, info, warning, error);
         var otherSection = AssertExtensions.RoundTripSerialize(section);

         Assert.AreEqual(section.Name, otherSection.Name);
         Assert.AreEqual(section.StartTime, otherSection.StartTime);
         Assert.AreEqual(section.EndTime, otherSection.EndTime);
         Assert.AreEqual(section.WasSuccessful, otherSection.WasSuccessful);

         Assert.That(
             otherSection.InfoMessages(),
             Is.EquivalentTo(section.InfoMessages()));
         Assert.That(
             otherSection.WarningMessages(),
             Is.EquivalentTo(section.WarningMessages()));
         Assert.That(
             otherSection.ErrorMessages(),
             Is.EquivalentTo(section.ErrorMessages()));
      }

      [Test]
      public void CreateWithEmptyName()
      {
         Assert.Throws<ArgumentException>(
            () => new TestSection(
               string.Empty,
               DateTimeOffset.Now,
               DateTimeOffset.Now,
               true,
               null,
               new List<DateBasedTestInformation>(),
               new List<DateBasedTestInformation>()));
      }

      [Test]
      public void Create()
      {
         var name = "someName";
         var start = DateTimeOffset.Now;
         var end = start.AddSeconds(10);
         bool success = false;
         var info = new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(1), "info") };
         var warning = new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(2), "warning") };
         var error = new List<DateBasedTestInformation> { new DateBasedTestInformation(start.AddSeconds(3), "error") };

         var section = new TestSection(name, start, end, success, info, warning, error);

         Assert.AreEqual(name, section.Name);
         Assert.AreEqual(start, section.StartTime);
         Assert.AreEqual(end, section.EndTime);
         Assert.AreEqual(success, section.WasSuccessful);

         Assert.That(
             section.InfoMessages(),
             Is.EquivalentTo(info));
         Assert.That(
             section.WarningMessages(),
             Is.EquivalentTo(warning));
         Assert.That(
             section.ErrorMessages(),
             Is.EquivalentTo(error));
      }
   }
}
