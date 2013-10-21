//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Executor.Properties;
using Sherlock.Shared.Core;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Provides methods for the transfer of report files.
    /// </summary>
    internal sealed class TransferTestReportDataCommands : ITransferTestReportDataCommands
    {
        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The function that handles the download of data from a remote endpoint.
        /// </summary>
        private readonly DownloadDataFromRemoteEndpoints m_DataDownload;

        /// <summary>
        /// The object that sends commands to remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_RemoteCommands;

        /// <summary>
        /// The object that stores links to all the files that should be uploaded.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// The object that stores information about the currently active test.
        /// </summary>
        private readonly ActiveTestInformation m_TestInformation;

        /// <summary>
        /// The object that stores information about the host.
        /// </summary>
        private readonly HostInformationStorage m_HostInformation;

        /// <summary>
        /// The directory in which all data files are stored.
        /// </summary>
        private readonly string m_StorageDirectory;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferTestReportDataCommands"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="dataDownload">The function that handles the download of data from a remote endpoint.</param>
        /// <param name="remoteCommands">The object that sends commands to remote endpoints.</param>
        /// <param name="uploads">The object that stores links to all the files that should be uploaded.</param>
        /// <param name="testInformation">The object that stores information about the currently active test.</param>
        /// <param name="hostInformation">The object that stores information about the host.</param>
        /// <param name="storageDirectory">The directory in which all the report files are stored.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataDownload"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="remoteCommands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hostInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageDirectory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TransferTestReportDataCommands(
            IFileSystem fileSystem, 
            DownloadDataFromRemoteEndpoints dataDownload, 
            ISendCommandsToRemoteEndpoints remoteCommands, 
            IStoreUploads uploads, 
            ActiveTestInformation testInformation,
            HostInformationStorage hostInformation, 
            string storageDirectory, 
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => dataDownload);
                Lokad.Enforce.Argument(() => remoteCommands);
                Lokad.Enforce.Argument(() => uploads);
                Lokad.Enforce.Argument(() => testInformation);
                Lokad.Enforce.Argument(() => hostInformation);
                Lokad.Enforce.Argument(() => storageDirectory);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_FileSystem = fileSystem;
            m_DataDownload = dataDownload;
            m_RemoteCommands = remoteCommands;
            m_Uploads = uploads;
            m_TestInformation = testInformation;
            m_HostInformation = hostInformation;
            m_StorageDirectory = storageDirectory;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Prepares the report files for upload.
        /// </summary>
        /// <param name="testStepIndex">The index of the test step for which the report files are being uploaded.</param>
        /// <param name="callingEndpoint">The ID of the endpoint that called the method.</param>
        /// <param name="token">The upload token used to register the report files that need to be uploaded.</param>
        /// <returns>An upload token that can be used to download the desired files.</returns>
        public Task PrepareReportFilesForTransfer(int testStepIndex, EndpointId callingEndpoint, UploadToken token)
        {
            m_Diagnostics.Log(
                LevelToLog.Trace,
                ExecutorServiceConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TransferingTestStepReportFiles_WithTestStep,
                    testStepIndex));

            var filePath = m_FileSystem.Path.Combine(
                m_StorageDirectory,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.zip",
                    Guid.NewGuid().ToString("D")));
            var fileStream = m_DataDownload(callingEndpoint, token, filePath);
            var uploadTask = fileStream.ContinueWith(
                file =>
                {
                    try
                    {
                        // Upload to the host here
                        var hostToken = m_Uploads.Register(file.Result.FullName);
                        if (!m_RemoteCommands.HasCommandFor(m_HostInformation.Id, typeof(IStoreTestReportDataCommands)))
                        {
                            throw new MissingCommandSetException();
                        }

                        var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
                        var command = m_RemoteCommands.CommandsFor<IStoreTestReportDataCommands>(m_HostInformation.Id);
                        var task = command.PrepareReportFilesForTransfer(m_TestInformation.TestId, testStepIndex, id, hostToken);
                        task.Wait();
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Error,
                            ExecutorServiceConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_TransferingTestStepReportFilesFailed_WithException,
                                e));

                        throw;
                    }
                });

            return uploadTask;
        }
    }
}
