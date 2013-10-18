//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Communication;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Stores information about the host service that the current service is connected to.
    /// </summary>
    internal sealed class HostInformationStorage
    {
        /// <summary>
        /// Gets or sets the endpoint ID of the host.
        /// </summary>
        public EndpointId Id
        {
            get;
            set;
        }
    }
}
