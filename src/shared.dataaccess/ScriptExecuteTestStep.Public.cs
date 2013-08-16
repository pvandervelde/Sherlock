//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public API part for the script execute test step class.
    /// </content>
    [Serializable]
    public sealed partial class ScriptExecuteTestStep
    {
        /// <summary>
        /// Gets or sets the script language that the script belonging to the current step is written in.
        /// </summary>
        public ScriptLanguage ScriptLanguage
        {
            get
            {
                return (ScriptLanguage)Enum.Parse(typeof(ScriptLanguage), Language);
            }

            set
            {
                Language = value.ToString();
            }
        }
    }
}
