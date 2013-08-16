//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Defines the pointer sizes for different operating systems.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags",
        Justification = "The underlying numbers are used for database storage, not for flags.")]
    public enum OperatingSystemPointerSize
    {
        /// <summary>
        /// Defines an invalid pointer size.
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines a 32-bit pointer size.
        /// </summary>
        X86 = 32,

        /// <summary>
        /// Defines a 64-bit pointer size.
        /// </summary>
        X64 = 64,
    }
}
