//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Windows.Forms;
using Autofac;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Sherlock.Service.Master.Nuclei;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Master
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

        private static void RegisterTestSuitePackage(ContainerBuilder builder)
        {
            builder.Register(c => new TestSuitePackage())
                .As<ITestSuitePackage>();
        }

        private static void RegisterStorage(ContainerBuilder builder)
        {
            builder.Register(c => new ActiveTestStorage())
                .As<IStoreActiveTests>()
                .SingleInstance();
        }

        private static void RegisterTestController(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        Func<string, IReportBuilder, ITestSectionBuilder> func =
                            (s, reportBuilder) =>
                            {
                                Action<string, TestSection> action = (s1, section) => reportBuilder.AddToSection(
                                    s1,
                                    new List<TestSection>
                                        {
                                            section
                                        });
                                return ctx.Resolve<ITestSectionBuilder>(
                                    new TypedParameter(typeof(string), s),
                                    new TypedParameter(
                                        typeof(Action<string, TestSection>),
                                        action));
                            };

                        return new TestController(
                            c.Resolve<IConfiguration>(),
                            c.Resolve<IStoreActiveTests>(),
                            ctx.Resolve<IProvideTestingContext>,
                            c.Resolve<IEnumerable<IEnvironmentActivator>>(),
                            ctx.Resolve<ITestSuitePackage>,
                            c.Resolve<IFileSystem>(),
                            ctx.Resolve<IReportBuilder>,
                            func,
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IControlTests>()
                .SingleInstance();
        }

        private static void RegisterTestCycle(ContainerBuilder builder)
        {
            builder.Register(c => new TestCycle(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<IControlTests>(),
                    c.Resolve<IStoreActiveTests>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<ICycleTestsFromRequestToCompletion>()
                .SingleInstance();
        }

        private static void RegisterReports(ContainerBuilder builder)
        {
            builder.Register((c, p) => new TestSectionBuilder(
                  p.TypedAs<string>(),
                  p.TypedAs<Action<string, TestSection>>()))
               .As<ITestSectionBuilder>();

            builder.Register(c => new ReportBuilder())
               .As<IReportBuilder>();

            builder.Register(c => new HtmlReportTransformer())
               .As<IReportTransformer>();

            builder.Register(c => new XmlReportTransformer())
               .As<IReportTransformer>();
        }

        private static void RegisterPlugins(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new HypervEnvironmentActivator(
                            c.Resolve<IConfiguration>(),
                            c.Resolve<ISendCommandsToRemoteEndpoints>(),
                            c.Resolve<INotifyOfRemoteEndpointEvents>(),
                            c.Resolve<ManualEndpointDisconnection>(),
                            c.Resolve<IStoreUploads>(),
                            c.Resolve<SystemDiagnostics>(),
                            id => ctx.Resolve<IProvideEnvironmentContext>().Machine(id));
                    })
                .As<IEnvironmentActivator>();

            builder.Register(c => new PhysicalMachineEnvironmentActivator(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<ISendCommandsToRemoteEndpoints>(),
                    c.Resolve<INotifyOfRemoteEndpointEvents>(),
                    c.Resolve<ManualEndpointDisconnection>(),
                    c.Resolve<IStoreUploads>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IEnvironmentActivator>();
        }

        /// <summary>
        /// Creates the DI container for the application.
        /// </summary>
        /// <param name="context">The application context that controls when the application will terminate.</param>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer(ApplicationContext context)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterInstance(context)
                    .As<ApplicationContext>()
                    .ExternallyOwned()
                    .SingleInstance();

                builder.RegisterModule(new NucleiModule());
                builder.RegisterModule(new DataAccessModule());

                builder.RegisterModule(
                    new CommunicationModule(
                        new List<CommunicationSubject>
                            {
                                CommunicationSubjects.TestScheduling,
                                CommunicationSubjects.TestTransfer,
                            },
                        new List<ChannelType>
                            {
                                ChannelType.TcpIP,
                            }, 
                        true));
                RegisterFileSystem(builder);
                RegisterTestSuitePackage(builder);
                RegisterStorage(builder);
                RegisterTestController(builder);
                RegisterTestCycle(builder);
                RegisterReports(builder);
                RegisterPlugins(builder);
            }

            return builder.Build();
        }
    }
}
