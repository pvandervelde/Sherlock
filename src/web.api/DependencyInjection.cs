//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Nuclei.Configuration;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        private static void RegisterTestSuitePackage(ContainerBuilder builder)
        {
            builder.Register(c => new TestSuitePackage())
                .As<ITestSuitePackage>();
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => !t.IsAbstract && typeof(ApiController).IsAssignableFrom(t))
                .InstancePerMatchingLifetimeScope(AutofacWebApiDependencyResolver.ApiRequestTag);

            // builder.RegisterControllers(typeof(WebApiApplication).Assembly);
        }

        /// <summary>
        /// Creates the DI container for the machine.
        /// </summary>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new DataAccessModule());

                builder.Register(c => new XmlConfiguration(
                        WebApiConfigurationKeys.ToCollection().ToList(),
                        WebApiConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>()
                    .SingleInstance();

                RegisterTestSuitePackage(builder);
                RegisterControllers(builder);
            }

            return builder.Build();
        }
    }
}
