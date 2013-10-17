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
    /// Defines the commands used to upload the report files for a specific test.
    /// </summary>
    public interface ITransferTestReportDataCommand : ICommandSet
    {
        /// <summary>
        /// Prepares the report files for upload.
        /// </summary>
        /// <param name="testStepIndex">The index of the test step for which the report files are being uploaded.</param>
        /// <param name="token">The upload token used to register the report files that need to be uploaded.</param>
        /// <returns>An upload token that can be used to download the desired files.</returns>
        Task PrepareReportFilesForTransfer(int testStepIndex, UploadToken token);
    }
}
