//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines a series of global constants.
    /// </summary>
    public static class GlobalConstants
    {
        /// <summary>
        /// The default value for the time between keep-alive messages send between the master service and the test environments.
        /// </summary>
        public const int DefaultKeepAliveCycleTimeInMilliseconds = 10000;

        /// <summary>
        /// The default value for the maximum amount of time the system will wait for a positive response to a network ping command.
        /// </summary>
        public const int DefaultPingTimeoutInMilliseconds = 1000;

        /// <summary>
        /// The default value for the maximum amount of time the system will wait for a sign-on to the network to succeed.
        /// </summary>
        public const int DefaultMaximumNetworkSignInTimeInMilliseconds = 600000;

        /// <summary>
        /// The default value for the time between consecutive ping operations.
        /// </summary>
        public const int DefaultPingCycleTimeInMilliseconds = 5000;

        /// <summary>
        /// The default value for the maximum number of times an environment can miss a ping cycle.
        /// </summary>
        public const int DefaultMaximumNumberOfCycleFailures = 10;

        /// <summary>
        /// The default value for the time between consecutive update checks.
        /// </summary>
        public const int DefaultUpdateCycleTimeInMilliseconds = 24 * 60 * 60 * 1000;

        /// <summary>
        /// The default value for the maximum time it takes to shut down a machine.
        /// </summary>
        public const int DefaultMaximumMachineShutdownTime = 2 * 60 * 1000;
    }
}
