//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the interface for strong-typed meta data describing an
    /// <see cref="IVersionedConfigurationReader"/>.
    /// </summary>
    internal interface IReaderVersionMetaData
    {
        /// <summary>
        /// Gets the version of the XML config file that the attached
        /// reader can handle.
        /// </summary>
        Version ReaderVersion
        {
            get;
        }
    }
}
