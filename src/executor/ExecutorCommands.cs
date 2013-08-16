//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Sherlock.Shared.Core;

namespace Sherlock.Executor
{
    internal sealed class ExecutorCommands : IExecutorCommands
    {
        /// <summary>
        /// Terminates the executor.
        /// </summary>
        /// <returns>A task that completes when the executor has received the message.</returns>
        public Task Terminate()
        {
            // This one is tricky. We need to be able to send out the success message
            // for the shutdown task but we can't do that if we shut down the app,
            // so we create a fake do-nothing task to send out the command succes result,
            // then in the attached task we actually shut down. Draw back is that we 
            // can't report in that there is a problem but there is nothing we can do 
            // about that.
            var task = Task.Factory.StartNew(
                () =>
                {
                    // Do nothing here. We just need a task to return.
                });

            task.ContinueWith(
                t =>
                {
                    Thread.Sleep(500);

                    // Terminate the app with extreme prejudice
                    Environment.Exit(Constants.ApplicationTerminatedExitCode);
                });

            return task;
        }
    }
}
