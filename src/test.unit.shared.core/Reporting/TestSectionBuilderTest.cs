//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
   [TestFixture]
   [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
   public sealed class TestSectionBuilderTest
   {
      [Test]
      public void CreateWithEmptySectionName()
      {
         Assert.Throws<ArgumentException>(() => new TestSectionBuilder(string.Empty, (s, t) => { }));
      }

      [Test]
      public void InitializeWithEmptyName()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         Assert.Throws<ArgumentException>(() => builder.Initialize(string.Empty));
      }

      [Test]
      public void AddInformationMessageWithEmptyText()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         builder.Initialize("someName");

         Assert.Throws<ArgumentException>(() => builder.AddInformationMessage(string.Empty));
      }

      [Test]
      public void AddInformationMessageWithNonInitializedBuilder()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         Assert.Throws<CannotAddMessageToUninitializedTestSectionException>(() => builder.AddInformationMessage("SomeText"));
      }

      [Test]
      public void AddWarningMessageWithEmptyText()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         builder.Initialize("someName");

         Assert.Throws<ArgumentException>(() => builder.AddWarningMessage(string.Empty));
      }

      [Test]
      public void AddWarningMessageWithNonInitializedBuilder()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         Assert.Throws<CannotAddMessageToUninitializedTestSectionException>(() => builder.AddWarningMessage("SomeText"));
      }

      [Test]
      public void AddErrorMessageWithEmptyText()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         builder.Initialize("someName");

         Assert.Throws<ArgumentException>(() => builder.AddErrorMessage(string.Empty));
      }

      [Test]
      public void AddErrorMessageWithNonInitializedBuilder()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         Assert.Throws<CannotAddMessageToUninitializedTestSectionException>(() => builder.AddErrorMessage("SomeText"));
      }

      [Test]
      public void FinalizeAndStoreWithUninitializedBuilder()
      {
         var builder = new TestSectionBuilder("section", (s, t) => { });
         Assert.Throws<CannotFinalizeAnUninitializedTestSectionException>(() => builder.FinalizeAndStore(false));
      }

      [Test]
      public void FinalizeAndStore()
      {
         var outputSectionName = string.Empty;
         TestSection result = null;
         Action<string, TestSection> onBuild =
            (s, t) =>
            {
               outputSectionName = s;
               result = t;
            };

         var sectionName = "section";
         var testSectionName = "someTest";
         var builder = new TestSectionBuilder(sectionName, onBuild);
         builder.Initialize(testSectionName);

         var information = "information";
         builder.AddInformationMessage(information);

         var warning = "warning";
         builder.AddWarningMessage(warning);

         var error = "error";
         builder.AddErrorMessage(error);

         builder.FinalizeAndStore(true);

         Assert.AreSame(sectionName, outputSectionName);
         Assert.AreSame(testSectionName, result.Name);
         Assert.IsTrue(result.WasSuccessful);
         Assert.AreSame(information, result.InfoMessages().First().Information);
         Assert.AreSame(warning, result.WarningMessages().First().Information);
         Assert.AreSame(error, result.ErrorMessages().First().Information);
      }
   }
}
