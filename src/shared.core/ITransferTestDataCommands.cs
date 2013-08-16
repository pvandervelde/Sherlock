//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nuclei.Communication;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the commands used to upload the files required to run a specific test.
    /// </summary>
    public interface ITransferTestDataCommands : ICommandSet
    {
        /// <summary>
        /// Returns the collection of test steps that should be executed.
        /// </summary>
        /// <returns>The collection of test steps that should be executed.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "It's a task that returns a strongly typed list.")]
        Task<List<TestStep>> TestCase();

        /// <summary>
        /// Returns a collection containing the parameters that describe the environment for the tests.
        /// </summary>
        /// <returns>A collection containing the environment parameters.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "It's a task that returns a strongly typed list.")]
        Task<List<InputParameter>> EnvironmentParameters();

        /// <summary>
        /// Prepares the test files for upload.
        /// </summary>
        /// <returns>An upload token that can be used to download the desired files.</returns>
        Task<UploadToken> PrepareTestFilesForTransfer();
    }
}
