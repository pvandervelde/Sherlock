//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// The constants used in the master service.
    /// </summary>
    internal static class MasterServiceConstants
    {
        /// <summary>
        /// The name of the configuration section that describes the configuration of the application.
        /// </summary>
        public const string ConfigurationSectionApplicationSettings = "masterservice";

        /// <summary>
        /// The prefix used for each log message.
        /// </summary>
        public const string LogPrefix = "Sherlock.Service.Master";

        /// <summary>
        /// The default value that indicates if an environment should be terminated if the test running
        /// on that environment completed but failed.
        /// </summary>
        public const bool DefaultShouldTerminateEnvironmentOnFailedTest = true;
    }
}
