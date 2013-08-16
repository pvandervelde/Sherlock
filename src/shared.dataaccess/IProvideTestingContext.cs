//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Defines the interface for objects that provide access to a testing context.
    /// </summary>
    public interface IProvideTestingContext : IProvideTestContext, IProvideEnvironmentContext
    {
    }
}
