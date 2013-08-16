//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO.Abstractions;
using Autofac;
using NAdoni;
using Sherlock.Service.Nuclei;

namespace Sherlock.Service
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        private static void RegisterFileSystem(ContainerBuilder builder)
        {
            builder.Register(c => new FileSystem())
                .As<IFileSystem>()
                .SingleInstance();
        }

        /// <summary>
        /// Creates the DI container for the application.
        /// </summary>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new NucleiModule());
                builder.Register(c => new Updater())
                    .As<Updater>()
                    .SingleInstance();

                RegisterFileSystem(builder);
            }

            return builder.Build();
        }
    }
}
