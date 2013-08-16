//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Linq;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the interface for objects that create an <see cref="TestStepDescription"/> object.
    /// </summary>
    internal interface IConstructTestSteps
    {
        /// <summary>
        /// Gets the contract that indicates what kind of input data this constructor can
        /// process.
        /// </summary>
        string Contract
        {
            get;
        }

        /// <summary>
        /// Constructs a test step from the given data.
        /// </summary>
        /// <param name="input">The XML element that contains the data for the test step.</param>
        /// <returns>The newly created test step.</returns>
        TestStepDescription Construct(XElement input);
    }
}
