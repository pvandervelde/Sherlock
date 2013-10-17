//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Sherlock.Executor
{
    /// <summary>
    /// A delegate that is used to upload test step result files to the host for inclusion in the test report.
    /// </summary>
    /// <param name="stepIndex">The index or order of the test step.</param>
    /// <param name="filesToUpload">The collection containing the files that should be uploaded and their original base directory.</param>
    internal delegate void UploadReportFilesForTestStep(int stepIndex, IDictionary<FileInfo, DirectoryInfo> filesToUpload);
}
