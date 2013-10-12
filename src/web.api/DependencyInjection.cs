//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;
using Autofac;
using Autofac.Integration.WebApi;
using NLog;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "web.api.info.log";

        private static void RegisterDiagnostics(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var loggers = c.Resolve<IEnumerable<ILogger>>();
                    Action<LevelToLog, string> action = (p, s) =>
                    {
                        var msg = new LogMessage(p, s);
                        foreach (var logger in loggers)
                        {
                            try
                            {
                                logger.Log(msg);
                            }
                            catch (NLogRuntimeException)
                            {
                                // Ignore it and move on to the next logger.
                            }
                        }
                    };

                    Profiler profiler = null;
                    if (c.IsRegistered<Profiler>())
                    {
                        profiler = c.Resolve<Profiler>();
                    }

                    return new SystemDiagnostics(action, profiler);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), @"..\App_Data", DefaultInfoFileName),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();
        }

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

                builder.Register(c => new NucleiBasedTraceWriter(
                        c.Resolve<SystemDiagnostics>()))
                    .As<ITraceWriter>()
                    .SingleInstance();

                RegisterLoggers(builder);
                RegisterDiagnostics(builder);
                RegisterTestSuitePackage(builder);
                RegisterControllers(builder);
            }

            return builder.Build();
        }
    }
}
