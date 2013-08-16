//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the delegate that is used for storing file data for a given test step in a given environment.
    /// </summary>
    /// <param name="environment">The name of the environment.</param>
    /// <param name="stepOrder">The name of the test step.</param>
    /// <param name="storedPath">The path under which the data should be stored.</param>
    /// <param name="filePath">The full path to the file that should be stored.</param>
    public delegate void StoreFileDataForEnvironment(string environment, int stepOrder, string storedPath, string filePath);
}
