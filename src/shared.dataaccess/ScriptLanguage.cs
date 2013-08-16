//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Defines the script languages that are supported by the script execution test step.
    /// </summary>
    public enum ScriptLanguage
    { 
        /// <summary>
        /// No script language is defined for the current test step.
        /// </summary>
        None,

        /// <summary>
        /// The current test step is a Powershell script.
        /// </summary>
        Powershell,
    }
}
