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
    /// Defines the commands used to store the report files for a specific test.
    /// </summary>
    public interface IStoreTestReportDataCommands : ICommandSet
    {
        /// <summary>
        /// Prepares the report files for upload.
        /// </summary>
        /// <param name="testId">The ID of the test to which the files belong.</param>
        /// <param name="testStepIndex">The index of the test step for which the report files are being uploaded.</param>
        /// <param name="callingEndpoint">The ID of the endpoint that called the method.</param>
        /// <param name="token">The upload token used to register the report files that need to be uploaded.</param>
        /// <returns>An upload token that can be used to download the desired files.</returns>
        Task PrepareReportFilesForTransfer(int testId, int testStepIndex, EndpointId callingEndpoint, UploadToken token);
    }
}
