//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
   [TestFixture]
   [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
   public sealed class ReportTestSectionEventArgsTest
   {
      [Test]
      public void CreateWithNullSection()
      {
         Assert.Throws<ArgumentNullException>(() => new ReportTestSectionEventArgs(null));
      }

      [Test]
      public void Create()
      {
         var section = new TestSection(
            "SomeName",
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddSeconds(10),
            true,
            new List<DateBasedTestInformation>(),
            new List<DateBasedTestInformation>(),
            new List<DateBasedTestInformation>());

         var args = new ReportTestSectionEventArgs(section);
         Assert.AreSame(section, args.Section);
      }
   }
}
