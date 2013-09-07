//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Executor
{
    /// <summary>
    /// A delegate that is used to retrieve the path of the directory that contains the test files for 
    /// the given test step.
    /// </summary>
    /// <param name="stepIndex">The name of the test step.</param>
    /// <returns>The full path to the directory that contains the files for the current test step.</returns>
    internal delegate string RetrieveFileDataForTestStep(int stepIndex);
}
