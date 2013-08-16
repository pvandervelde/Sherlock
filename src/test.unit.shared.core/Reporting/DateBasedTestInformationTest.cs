//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Core.Testing;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
   [TestFixture]
   [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
   public sealed class DateBasedTestInformationTest : EqualityContractVerifier<DateBasedTestInformation>
   {
       private readonly IEnumerable<DateBasedTestInformation> m_DistinctInstances
            = new List<DateBasedTestInformation> 
                     {
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "a"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(2, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "b"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 3, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "c"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 4, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "d"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 5, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "e"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 4, 6, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "f"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 4, 5, 7, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "g"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 8, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            "h"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, new JapaneseCalendar(), new TimeSpan()),
                            "i"),
                        new DateBasedTestInformation(
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan(12, 0, 0)),
                            "j"),
                     };

       private readonly DateBasedTestInformation m_First
           = new DateBasedTestInformation(
                new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                "a");

       private readonly DateBasedTestInformation m_Second
            = new DateBasedTestInformation(
                new DateTimeOffset(1, 2, 3, 4, 5, 6, 8, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                "b");

       public override IEnumerable<DateBasedTestInformation> DistinctInstances
       {
           get
           {
               return m_DistinctInstances;
           }
       }

       public override double UniformDistributionQualityLimit
       {
           get
           {
               return UniformDistributionQuality.Excellent;
           }
       }

       public override double CollisionProbabilityLimit
       {
           get
           {
               return CollisionProbability.VeryLow;
           }
       }

       public override DateBasedTestInformation Copy(DateBasedTestInformation original)
       {
           return new DateBasedTestInformation(original.Time, original.Information);
       }

       public override DateBasedTestInformation FirstInstance
       {
           get
           {
               return m_First;
           }
       }

       public override DateBasedTestInformation SecondInstance
       {
           get
           {
               return m_Second;
           }
       }

       public override bool HasOperatorOverloads
       {
           get
           {
               return true;
           }
       }

      [Test]
      public void RoundTripSerialise()
      {
         var info = new DateBasedTestInformation(DateTimeOffset.Now, "a");
         var otherInfo = AssertExtensions.RoundTripSerialize(info);

         Assert.AreEqual(info, otherInfo);
      }
   }
}
