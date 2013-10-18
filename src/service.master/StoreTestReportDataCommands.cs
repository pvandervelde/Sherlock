//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Provides methods for the storage of report files.
    /// </summary>
    internal sealed class StoreTestReportDataCommands : IStoreTestReportDataCommands
    {
        /// <summary>
        /// The object that provides access to the file system.
        /// </summary>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The object that provides access to the configuration options for the application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The function that handles the download of data from a remote endpoint.
        /// </summary>
        private readonly DownloadDataFromRemoteEndpoints m_DataDownload;

        /// <summary>
        /// The object that stores information about all the active tests.
        /// </summary>
        private readonly IStoreActiveTests m_ActiveTests;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Inititalizes a new instance of the <see cref="StoreTestReportDataCommands"/> class.
        /// </summary>
        /// <param name="fileSystem">The object that provides access to the file system.</param>
        /// <param name="configuration">The object that provides access to the application configuration.</param>
        /// <param name="activeTests">The object that stores information about all the active tests.</param>
        /// <param name="dataDownload">The function that handles the download of data from a remote endpoint.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        public StoreTestReportDataCommands(
            IFileSystem fileSystem, 
            IConfiguration configuration, 
            IStoreActiveTests activeTests, 
            DownloadDataFromRemoteEndpoints dataDownload, 
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => activeTests);
                Lokad.Enforce.Argument(() => dataDownload);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_FileSystem = fileSystem;
            m_Configuration = configuration;
            m_ActiveTests = activeTests;
            m_DataDownload = dataDownload;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Prepares the report files for upload.
        /// </summary>
        /// <param name="testId">The ID of the test to which the files belong.</param>
        /// <param name="testStepIndex">The index of the test step for which the report files are being uploaded.</param>
        /// <param name="callingEndpoint">The ID of the endpoint that called the method.</param>
        /// <param name="token">The upload token used to register the report files that need to be uploaded.</param>
        /// <returns>An upload token that can be used to download the desired files.</returns>
        public Task PrepareReportFilesForTransfer(int testId, int testStepIndex, EndpointId callingEndpoint, UploadToken token)
        {
            m_Diagnostics.Log(
                LevelToLog.Trace,
                MasterServiceConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Messages_TransferingTestStepReportFiles_WithTestAndTestStep,
                    testId,
                    testStepIndex));

            var downloadDirectory = TestHelpers.StoragePathForReportFiles(testId, m_Configuration, m_FileSystem);
            var fileStream = m_DataDownload(callingEndpoint, token, downloadDirectory);
            var uploadTask = fileStream.ContinueWith(
                file =>
                {
                    var testPackage = new TestStepPackage(testStepIndex);
                    testPackage.LoadAndUnpack(file.Result.FullName, downloadDirectory);

                    var notifications = m_ActiveTests.NotificationsFor(testId);
                    foreach (var notification in notifications)
                    {
                        foreach (var storedFile in testPackage.StoredFiles())
                        {
                            var relativePath = storedFile.FullName.Substring(downloadDirectory.Length).TrimStart(Path.DirectorySeparatorChar);
                            notification.StoreReportFile(storedFile.FullName, relativePath);
                        }
                    }
                });

            return uploadTask;
        }
    }
}
