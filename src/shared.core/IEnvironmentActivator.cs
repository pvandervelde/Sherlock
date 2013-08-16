//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that create active environments from a specification.
    /// </summary>
    public interface IEnvironmentActivator
    {
        /// <summary>
        /// Gets the type of the environment specification that can be handled by the current activator.
        /// </summary>
        Type EnvironmentTypeToLoad
        { 
            get; 
        }

        /// <summary>
        /// Creates a new active environment based on the given specification.
        /// </summary>
        /// <param name="environment">The specification that provides the configuration for the active environment.</param>
        /// <param name="sectionBuilder">
        /// The object used to write information to the report about the starting and stopping of the environment.
        /// </param>
        /// <param name="onUnload">The action that is executed upon test completion.</param>
        /// <returns>A new active environment.</returns>
        IActiveEnvironment Load(MachineDescription environment, ITestSectionBuilder sectionBuilder, Action<string> onUnload);
    }
}
