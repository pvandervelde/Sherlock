//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Autofac;

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Provides the component registrations for the data access layer.
    /// </summary>
    public sealed class DataAccessModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new SherlockContext())
                .As<IProvideEnvironmentContext>()
                .As<IProvideTestContext>()
                .As<IProvideTestingContext>();
        }
    }
}
