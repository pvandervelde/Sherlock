//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the interface for objects that read configuration files that describe a test.
    /// </summary>
    internal interface IVersionedConfigurationReader
    {
        /// <summary>
        /// Reads the configuration from the XML document.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>A new configuration information object.</returns>
        ConfigurationInfo Read(XDocument xmlDocument);
    }
}
