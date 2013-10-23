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
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
   [TestFixture]
   [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
   public sealed class DateBasedTestInformationTest : EqualityContractVerifierTest
   {
       private sealed class DateBasedTestInformationEqualityContractVerifier : EqualityContractVerifier<DateBasedTestInformation>
       {
           private readonly DateBasedTestInformation m_First
           = new DateBasedTestInformation(
                new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                "a");

           private readonly DateBasedTestInformation m_Second
                = new DateBasedTestInformation(
                    new DateTimeOffset(1, 2, 3, 4, 5, 6, 8, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                    "b");

           protected override DateBasedTestInformation Copy(DateBasedTestInformation original)
           {
               return new DateBasedTestInformation(original.Time, original.Information);
           }

           protected override DateBasedTestInformation FirstInstance
           {
               get
               {
                   return m_First;
               }
           }

           protected override DateBasedTestInformation SecondInstance
           {
               get
               {
                   return m_Second;
               }
           }

           protected override bool HasOperatorOverloads
           {
               get
               {
                   return true;
               }
           }
       }

       private sealed class DateBasedTestInformationHashcodeContractVerfier : HashcodeContractVerifier
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

           protected override IEnumerable<int> GetHashcodes()
           {
               return m_DistinctInstances.Select(i => i.GetHashCode());
           }
       }

       private readonly DateBasedTestInformationHashcodeContractVerfier m_HashcodeVerifier 
           = new DateBasedTestInformationHashcodeContractVerfier();

       private readonly DateBasedTestInformationEqualityContractVerifier m_EqualityVerifier
           = new DateBasedTestInformationEqualityContractVerifier();

       protected override HashcodeContractVerifier HashContract
       {
           get
           {
               return m_HashcodeVerifier;
           }
       }

       protected override IEqualityContractVerifier EqualityContract
       {
           get
           {
               return m_EqualityVerifier;
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
