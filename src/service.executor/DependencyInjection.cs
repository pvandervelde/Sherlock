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
using Sherlock.Service.Executor.Nuclei;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        private static void RegisterFileSystem(ContainerBuilder builder)
        {
            builder.Register(c => new FileSystem())
                .As<IFileSystem>();
        }

        private static void RegisterTestInformation(ContainerBuilder builder)
        {
            builder.Register(c => new ActiveTestInformation())
                .SingleInstance();
        }

        private static void RegisterReports(ContainerBuilder builder)
        {
            builder.Register((c, p) => new TestSectionBuilder(
                  p.TypedAs<string>(),
                  p.TypedAs<Action<string, TestSection>>()))
               .As<ITestSectionBuilder>();
        }

        private static void RegisterNotifications(ContainerBuilder builder)
        {
            builder.Register(c => new TestExecutionNotifications())
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<INotificationSendersCollection>();
                        collection.Store(typeof(ITestExecutionNotifications), a.Instance);
                    })
                .As<ITestExecutionNotificationsInvoker>()
                .As<ITestExecutionNotifications>()
                .As<INotificationSet>()
                .SingleInstance();
        }

        private static void RegisterCommands(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    Func<string, ITestSectionBuilder> func =
                        s =>
                        {
                            var notifications = ctx.Resolve<ITestExecutionNotificationsInvoker>();
                            Action<string, TestSection> action = notifications.RaiseOnExecutionProgress;
                            return ctx.Resolve<ITestSectionBuilder>(
                                new TypedParameter(typeof(string), s),
                                new TypedParameter(typeof(Action<string, TestSection>), action));
                        };

                    return new TestStepExecutionCommands(
                        c.Resolve<IFileSystem>(),
                        c.Resolve<ICommunicationLayer>(),
                        c.Resolve<ISendCommandsToRemoteEndpoints>(),
                        c.Resolve<INotifyOfRemoteEndpointEvents>(),
                        c.Resolve<DownloadDataFromRemoteEndpoints>(),
                        c.Resolve<ITestExecutionNotificationsInvoker>(),
                        func,
                        c.Resolve<IConfiguration>(),
                        c.Resolve<ActiveTestInformation>(),
                        c.Resolve<SystemDiagnostics>());
                })
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IExecuteTestStepsCommands), a.Instance);
                    })
                .As<IExecuteTestStepsCommands>()
                .As<ICommandSet>()
                .SingleInstance();

            builder.Register(c => new TransferTestDataCommands(
                    c.Resolve<IStoreUploads>(),
                    c.Resolve<ActiveTestInformation>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(ITransferTestDataCommands), a.Instance);
                    })
                .As<ITransferTestDataCommands>()
                .As<ICommandSet>()
                .SingleInstance();
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

                builder.RegisterModule(new UtilitiesModule());
                builder.RegisterModule(
                    new CommunicationModule(
                        new List<CommunicationSubject>
                            {
                                CommunicationSubjects.TestTransfer,
                                CommunicationSubjects.TestExecution,
                            },
                        new List<ChannelType>
                            {
                                ChannelType.NamedPipe,
                                ChannelType.TcpIP,
                            }, 
                        true));

                RegisterFileSystem(builder);
                RegisterReports(builder);
                RegisterTestInformation(builder);
                RegisterNotifications(builder);
                RegisterCommands(builder);
            }

            return builder.Build();
        }
    }
}
