//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Nuclei.Communication;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines operation commands for test executors.
    /// </summary>
    public interface IExecutorCommands : ICommandSet
    {
        /// <summary>
        /// Terminates the executor.
        /// </summary>
        /// <returns>A task that completes when the executor has received the message.</returns>
        Task Terminate();
    }
}
