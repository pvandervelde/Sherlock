//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Defines a data storage object that maps an information string to a date and time on
   /// which the information was generated.
   /// </summary>
   [Serializable]
   public sealed class DateBasedTestInformation : IEquatable<DateBasedTestInformation>
   {
      /// <summary>
      /// Implements the operator ==.
      /// </summary>
      /// <param name="first">The first object.</param>
      /// <param name="second">The second object.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(DateBasedTestInformation first, DateBasedTestInformation second)
      {
         // Check if first and second are null references by using ReferenceEquals because
         // we overload the == operator.
         if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
         {
            return true;
         }

         // Check if first is a null reference by using ReferenceEquals because
         // we overload the == operator. If first isn't actually null then
         // we get an infinite loop where we're constantly trying to compare to null.
         if (ReferenceEquals(first, null))
         {
            return false;
         }

         return first.Equals(second);
      }

      /// <summary>
      /// Implements the operator !=.
      /// </summary>
      /// <param name="first">The first object.</param>
      /// <param name="second">The second object.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(DateBasedTestInformation first, DateBasedTestInformation second)
      {
         // Check if first and second are null references by using ReferenceEquals because
         // we overload the == operator.
         if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
         {
            return false;
         }

         // Check if first is a null reference by using ReferenceEquals because
         // we overload the == operator. If first isn't actually null then
         // we get an infinite loop where we're constantly trying to compare to null.
         if (ReferenceEquals(first, null))
         {
            return true;
         }

         return !first.Equals(second);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="DateBasedTestInformation"/> class.
      /// </summary>
      /// <param name="time">The date and time on which the information was created.</param>
      /// <param name="information">The information.</param>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="information"/> is <see langword="null" />.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="information"/> is an empty string.
      /// </exception>
      public DateBasedTestInformation(DateTimeOffset time, string information)
      {
         {
             Lokad.Enforce.Argument(() => information);
             Lokad.Enforce.Argument(() => information, Lokad.Rules.StringIs.NotEmpty);
         }

         Time = time;
         Information = information;
      }

      /// <summary>
      /// Gets the date and time on which the information was created.
      /// </summary>
      public DateTimeOffset Time
      {
         get;
         private set;
      }

      /// <summary>
      /// Gets the information.
      /// </summary>
      public string Information
      {
         get;
         private set;
      }

      /// <summary>
      /// Indicates whether the current object is equal to another object of the same type.
      /// </summary>
      /// <param name="other">An object to compare with this object.</param>
      /// <returns>
      ///   <see langword="true" /> if the current object is equal to the <paramref name="other"/> parameter;
      ///   otherwise, <see langword="false" />.
      /// </returns>
      [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
      public bool Equals(DateBasedTestInformation other)
      {
         if (ReferenceEquals(this, other))
         {
            return true;
         }

         if (ReferenceEquals(other, null))
         {
            return false;
         }

         return Time.Equals(other.Time) && string.Equals(Information, other.Information, StringComparison.CurrentCulture);
      }

      /// <summary>
      /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
      /// </summary>
      /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
      /// <returns>
      ///   <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
      ///   otherwise, <see langword="false"/>.
      /// </returns>
      [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
      public override bool Equals(object obj)
      {
         if (ReferenceEquals(this, obj))
         {
            return true;
         }

         if (ReferenceEquals(obj, null))
         {
            return false;
         }

         var info = obj as DateBasedTestInformation;
         if (!ReferenceEquals(info, null))
         {
            return Equals(info);
         }

         return false;
      }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>
      /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
      /// </returns>
      public override int GetHashCode()
      {
         // As obtained from the Jon Skeet answer to:
         //   http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
         // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
         //
         // Overflow is fine, just wrap
         unchecked
         {
            // Pick a random prime number
            int hash = 17;

            // Mash the hash together with yet another random prime number
            hash = (hash * 23) ^ Time.GetHashCode();
            hash = (hash * 23) ^ Information.GetHashCode();

            return hash;
         }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString()
      {
         return string.Format(CultureInfo.InvariantCulture, "Date based information: {0} - {1}", Time, Information);
      }
   }
}
