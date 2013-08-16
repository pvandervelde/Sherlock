//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nuclei.Communication;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Provides methods for test data transfer.
    /// </summary>
    internal sealed class TransferTestDataCommands : ITransferTestDataCommands
    {
        /// <summary>
        /// The object that stores the files that are waiting for upload.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// The object that stores information about the currently active test.
        /// </summary>
        private readonly ActiveTestInformation m_TestInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferTestDataCommands"/> class.
        /// </summary>
        /// <param name="uploads">The object that stores the files waiting for upload.</param>
        /// <param name="testInformation">The object that stores information about the currently active test.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testInformation"/> is <see langword="null" />.
        /// </exception>
        public TransferTestDataCommands(IStoreUploads uploads, ActiveTestInformation testInformation)
        {
            {
                Lokad.Enforce.Argument(() => uploads);
                Lokad.Enforce.Argument(() => testInformation);
            }

            m_Uploads = uploads;
            m_TestInformation = testInformation;
        }

        /// <summary>
        /// Returns the collection of test steps that should be executed.
        /// </summary>
        /// <returns>The collection of test steps that should be executed.</returns>
        public Task<List<TestStep>> TestCase()
        {
            return Task<List<TestStep>>.Factory.StartNew(() => m_TestInformation.TestSteps);
        }

        /// <summary>
        /// Returns a collection containing the parameters that describe the environment for the tests.
        /// </summary>
        /// <returns>A collection containing the environment parameters.</returns>
        public Task<List<InputParameter>> EnvironmentParameters()
        {
            return Task<List<InputParameter>>.Factory.StartNew(() => m_TestInformation.EnvironmentParameters);
        }

        /// <summary>
        /// Prepares the test files for upload.
        /// </summary>
        /// <returns>An upload token that can be used to download the desired files.</returns>
        public Task<UploadToken> PrepareTestFilesForTransfer()
        {
            return Task<UploadToken>.Factory.StartNew(() => m_Uploads.Register(m_TestInformation.TestPackage));
        }
    }
}
