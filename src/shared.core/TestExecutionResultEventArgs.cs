//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that stores the result status of a test execution.
    /// </summary>
    [Serializable]
    public sealed class TestExecutionResultEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the ID of the test.
        /// </summary>
        private readonly int m_Id;

        /// <summary>
        /// The status result of the test execution.
        /// </summary>
        private readonly TestExecutionResult m_Result;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExecutionResultEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        /// <param name="result">The status result of the test execution.</param>
        public TestExecutionResultEventArgs(int id, TestExecutionResult result)
        {
            m_Id = id;
            m_Result = result;
        }

        /// <summary>
        /// Gets the ID of the test.
        /// </summary>
        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the status result of the test execution.
        /// </summary>
        public TestExecutionResult Result
        {
            get
            {
                return m_Result;
            }
        }
    }
}
