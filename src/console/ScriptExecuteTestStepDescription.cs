//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores a description of a test step that executes a script.
    /// </summary>
    internal sealed class ScriptExecuteTestStepDescription : TestStepDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptExecuteTestStepDescription"/> class.
        /// </summary>
        /// <param name="environment">The name of the environment to which the current step belongs.</param>
        /// <param name="order">The index of the test step in the test sequence.</param>
        /// <param name="failureMode">The failure mode that describes what action should be taken if the current test step fails.</param>
        /// <param name="parameters">The collection containing the parameters for the current test step.</param>
        /// <param name="scriptLanguage">The name of the script language used to write the script that should be executed.</param>
        public ScriptExecuteTestStepDescription(
            string environment, 
            int order,
            string failureMode,
            IEnumerable<TestStepParameterDescription> parameters, 
            string scriptLanguage) 
            : base(environment, order, failureMode, parameters)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(scriptLanguage), "The script language name should not be an empty string.");
            }

            ScriptLanguage = scriptLanguage;
        }

        /// <summary>
        /// Gets the name of the script language that was used to write the script that should
        /// be executed by the current step.
        /// </summary>
        public string ScriptLanguage
        {
            get;
            private set;
        }
    }
}
