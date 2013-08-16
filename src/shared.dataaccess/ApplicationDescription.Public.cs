//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API for the application class.
    /// </content>
    public sealed partial class ApplicationDescription : IEquatable<ApplicationDescription>, IComparable<ApplicationDescription>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ApplicationDescription left, ApplicationDescription right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ApplicationDescription left, ApplicationDescription right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(ApplicationDescription left, ApplicationDescription right)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null))
            {
                return false;
            }

            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(ApplicationDescription left, ApplicationDescription right)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null))
            {
                return false;
            }

            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(ApplicationDescription left, ApplicationDescription right)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null))
            {
                return true;
            }

            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(ApplicationDescription left, ApplicationDescription right)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(left, null))
            {
                return true;
            }

            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently undergoing patching, i.e.
        /// adding all values from the database.
        /// </summary>
        internal bool IsPatching
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object has been patched, i.e. all values have been
        /// extracted from the database, or not.
        /// </summary>
        internal bool IsPatched
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID of the application.
        /// </summary>
        public int Id
        {
            get
            {
                return pk_ApplicationId;
            }

            set
            {
                pk_ApplicationId = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of the application.
        /// </summary>
        public Version Version
        {
            get
            {
                return new Version(VersionMajor, VersionMinor, VersionPatch, VersionBuild);
            }

            set
            {
                VersionMajor = value.Major;
                VersionMinor = value.Minor;
                VersionPatch = value.Build;
                VersionBuild = value.Revision;
            }
        }

        /// <summary>
        /// Determines whether the current software is the newer (i.e. higher version number) of
        /// the specified software.
        /// </summary>
        /// <param name="software">The software.</param>
        /// <returns>
        ///   <see langword="true"/> if the current software is the newer (i.e. higher version number) of
        /// the specified software; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        public bool IsNewerVersionOf(ApplicationDescription software)
        {
            if (ReferenceEquals(software, null))
            {
                return false;
            }

            if (Equals(software))
            {
                return false;
            }

            if (IsApplicationNameEqual(software))
            {
                return this > software;
            }

            return false;
        }

        private bool IsApplicationNameEqual(ApplicationDescription software)
        {
            return string.Equals(Name, software.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the current software is the newer or equal version (i.e. higher or equal
        /// version number) of the specified software.
        /// </summary>
        /// <param name="software">The software.</param>
        /// <returns>
        ///   <see langword="true"/> if the current software is the newer or equal version (i.e.
        ///   higher or equal version number) of the specified software; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        public bool IsNewerOrEqualVersionOf(ApplicationDescription software)
        {
            if (ReferenceEquals(software, null))
            {
                return false;
            }

            if (Equals(software))
            {
                return true;
            }

            if (IsApplicationNameEqual(software))
            {
                return this >= software;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the current software is the older (i.e. lower version number) of
        /// the specified software.
        /// </summary>
        /// <param name="software">The software.</param>
        /// <returns>
        ///   <see langword="true"/> if the current software is the lower (i.e. lower version number) of
        /// the specified software; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        public bool IsOlderVersionOf(ApplicationDescription software)
        {
            if (ReferenceEquals(software, null))
            {
                return false;
            }

            if (Equals(software))
            {
                return false;
            }

            if (IsApplicationNameEqual(software))
            {
                return this < software;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the current software is the older or equal version (i.e. lower or equal
        /// version number) of the specified software.
        /// </summary>
        /// <param name="software">The software.</param>
        /// <returns>
        ///   <see langword="true"/> if the current software is the lower version (i.e. lower or equal
        ///   version number) of the specified software; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        public bool IsOlderOrEqualVersionOf(ApplicationDescription software)
        {
            if (ReferenceEquals(software, null))
            {
                return false;
            }

            if (Equals(software))
            {
                return true;
            }

            if (IsApplicationNameEqual(software))
            {
                return this <= software;
            }

            return false;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return other has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(ApplicationDescription other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            var result = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            if (result == 0)
            {
                result = VersionMajor.CompareTo(other.VersionMajor);
            }

            if (result == 0)
            {
                result = VersionMinor.CompareTo(other.VersionMinor);
            }

            if (result == 0)
            {
                result = VersionPatch.CompareTo(other.VersionPatch);
            }

            if (result == 0)
            {
                result = VersionBuild.CompareTo(other.VersionBuild);
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ApplicationDescription other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return string.Equals(Name, other.Name) 
                && VersionMajor == other.VersionMajor 
                && VersionMinor == other.VersionMinor 
                && VersionPatch == other.VersionPatch 
                && VersionBuild == other.VersionBuild;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var applicationDescription = obj as ApplicationDescription;
            return (applicationDescription != null) && Equals(applicationDescription);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
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
                hash = (hash * 397) ^ Name.GetHashCode();
                hash = (hash * 397) ^ VersionMajor.GetHashCode();
                hash = (hash * 397) ^ VersionMinor.GetHashCode();
                hash = (hash * 397) ^ VersionPatch.GetHashCode();
                hash = (hash * 397) ^ VersionBuild.GetHashCode();

                return hash;
            }
        }
    }
}
