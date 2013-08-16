//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Defines the interface for objects that define a context that is stored in some fashion.
    /// </summary>
    public interface IProvideStoredContext
    {
        /// <summary>
        /// Stores all the changes via the underlying storage mechanism.
        /// </summary>
        void StoreChanges();
    }
}
