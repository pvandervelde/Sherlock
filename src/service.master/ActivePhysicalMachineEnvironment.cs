﻿//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication;
using Sherlock.Shared.Core;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines a proxy for an active physical machine that can be used to run tests on.
    /// </summary>
    internal sealed class ActivePhysicalMachineEnvironment : ActiveMachineEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivePhysicalMachineEnvironment"/> class.
        /// </summary>
        /// <param name="id">The ID of the environment.</param>
        /// <param name="terminateEnvironment">The action used to terminate the environment.</param>
        /// <param name="commands">The object that provides the commands used to communicate with the environment.</param>
        /// <param name="notifications">The object that provides notifications from the environment.</param>
        /// <param name="uploads">The object that tracks the files available for upload.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="terminateEnvironment"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        public ActivePhysicalMachineEnvironment(
            string id, 
            Action terminateEnvironment, 
            IExecuteTestStepsCommands commands, 
            ITestExecutionNotifications notifications,
            IStoreUploads uploads) 
            : base(id, terminateEnvironment, commands, notifications, uploads)
        {
        }
    }
}
