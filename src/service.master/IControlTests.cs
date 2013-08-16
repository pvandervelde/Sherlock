//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the interface for objects that control the execution of tests.
    /// </summary>
    internal interface IControlTests
    {
        /// <summary>
        /// Activates all the tests for which environments can be found.
        /// </summary>
        void ActivateTests();
    }
}
