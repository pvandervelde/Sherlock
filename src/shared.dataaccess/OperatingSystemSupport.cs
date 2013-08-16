//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Provides data used while handling operating systems.
    /// </summary>
    public static class OperatingSystemSupport
    {
        /// <summary>
        /// Defines the list of supported operating system cultures.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "CultureInfo is immutable.")]
        public static readonly IEnumerable<CultureInfo> Cultures
            = new List<CultureInfo> 
            { 
                new CultureInfo("en-NZ"),
                new CultureInfo("en-US"),
            };
    }
}
