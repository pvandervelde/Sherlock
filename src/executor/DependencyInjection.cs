//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using Autofac;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Sherlock.Executor.Nuclei;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Executor
{
    internal static class DependencyInjection
    {
        private static UploadReportFilesForTestStep BuildReportFileUploader(IComponentContext context, string storageDirectory, EndpointId hostId)
        {
            UploadReportFilesForTestStep uploader =
                (stepIndex, filesToUpload) =>
                {
                    if (filesToUpload.Count == 0)
                    {
                        return;
                    }

                    var path = Path.Combine(
                        storageDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "TestStepReport-{0}.zip",
                            stepIndex));
                    var package = new TestStepPackage(stepIndex);
                    foreach (var pair in filesToUpload)
                    {
                        var filePath = pair.Key.FullName;
                        var baseDirectory = pair.Value.FullName;

                        var relativePath = filePath.Substring(baseDirectory.Length).TrimStart(Path.DirectorySeparatorChar);
                        package.Add(filePath, relativePath);
                    }

                    package.PackTo(path);

                    var uploads = context.Resolve<IStoreUploads>();
                    var token = uploads.Register(path);

                    var hub = context.Resolve<ISendCommandsToRemoteEndpoints>();
                    if (!hub.HasCommandFor(hostId, typeof(ITransferTestReportDataCommands)))
                    {
                        throw new MissingCommandSetException();
                    }

                    var command = hub.CommandsFor<ITransferTestReportDataCommands>(hostId);

                    var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
                    Action toExecute =
                        () =>
                        {
                            var task = command.PrepareReportFilesForTransfer(
                                stepIndex,
                                id,
                                token);
                            task.Wait();
                        };
                    CommandSetGuard.GuardAgainstCommunicationFailure(toExecute);
                };

            return uploader;
        }

        private static void RegisterFileSystem(ContainerBuilder builder)
        {
            builder.Register(c => new FileSystem())
                .As<IFileSystem>()
                .SingleInstance();
        }

        private static void RegisterCore(ContainerBuilder builder, string storageDirectory)
        {
            builder.Register(c => new ConsoleApplicationRunner())
                .As<IRunConsoleApplications>()
                .SingleInstance();

            builder.Register((c, p) => new TestSectionBuilder(
                  p.TypedAs<string>(),
                  p.TypedAs<Action<string, TestSection>>()))
               .As<ITestSectionBuilder>();

            builder.Register(c => new TestEnvironmentPackage())
                .As<ITestEnvironmentPackage>();

            builder.Register<RetrieveFileDataForTestStep>(
                    c => index => Path.Combine(storageDirectory, index.ToString(CultureInfo.InvariantCulture)))
                .SingleInstance();
        }

        private static void RegisterCommands(ContainerBuilder builder)
        {
            builder.Register(c => new ExecutorCommands())
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IExecutorCommands), a.Instance);
                    })
                .As<IExecutorCommands>()
                .As<ICommandSet>()
                .SingleInstance();
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

        private static void RegisterTestStepProcessors(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var notifications = c.Resolve<ITestExecutionNotificationsInvoker>();
                        Action<string, TestSection> action = notifications.RaiseOnExecutionProgress;
                        return new ConsoleExecuteTestStepProcessor(
                            c.Resolve<RetrieveFileDataForTestStep>(),
                            c.Resolve<UploadReportFilesForTestStep>(),
                            c.Resolve<SystemDiagnostics>(),
                            c.Resolve<IRunConsoleApplications>(),
                            c.Resolve<IFileSystem>(),
                            c.Resolve<ITestSectionBuilder>(
                                new TypedParameter(
                                    typeof(string),
                                    "testing"),
                                new TypedParameter(
                                    typeof(Action<string, TestSection>),
                                    action)));
                    })
                .As<IProcessTestStep>();

            builder.Register(
                    c =>
                    {
                        var notifications = c.Resolve<ITestExecutionNotificationsInvoker>();
                        Action<string, TestSection> action = notifications.RaiseOnExecutionProgress;
                        return new MsiDeployTestStepProcessor(
                            c.Resolve<RetrieveFileDataForTestStep>(),
                            c.Resolve<UploadReportFilesForTestStep>(),
                            c.Resolve<SystemDiagnostics>(),
                            c.Resolve<IRunConsoleApplications>(),
                            c.Resolve<IFileSystem>(),
                            c.Resolve<ITestSectionBuilder>(
                                new TypedParameter(
                                    typeof(string),
                                    "setup"),
                                new TypedParameter(
                                    typeof(Action<string, TestSection>),
                                    action)));
                    })
                .As<IProcessTestStep>();

            builder.Register(
                c =>
                {
                    var notifications = c.Resolve<ITestExecutionNotificationsInvoker>();
                    Action<string, TestSection> action = notifications.RaiseOnExecutionProgress;
                    return new ScriptExecuteTestStepProcessor(
                        c.Resolve<RetrieveFileDataForTestStep>(),
                        c.Resolve<UploadReportFilesForTestStep>(),
                        c.Resolve<SystemDiagnostics>(),
                        c.Resolve<IFileSystem>(),
                        c.Resolve<ITestSectionBuilder>(
                            new TypedParameter(
                                typeof(string),
                                "testing"),
                            new TypedParameter(
                                typeof(Action<string, TestSection>),
                                action)));
                })
                .As<IProcessTestStep>();

            builder.Register(
                c =>
                {
                    var notifications = c.Resolve<ITestExecutionNotificationsInvoker>();
                    Action<string, TestSection> action = notifications.RaiseOnExecutionProgress;
                    return new XCopyDeployTestStepProcessor(
                        c.Resolve<RetrieveFileDataForTestStep>(),
                        c.Resolve<UploadReportFilesForTestStep>(),
                        c.Resolve<SystemDiagnostics>(),
                        c.Resolve<IFileSystem>(),
                        c.Resolve<ITestSectionBuilder>(
                            new TypedParameter(
                                typeof(string),
                                "setup"),
                            new TypedParameter(
                                typeof(Action<string, TestSection>),
                                action)));
                })
                .As<IProcessTestStep>();
        }

        /// <summary>
        /// Creates the DI container for the application.
        /// </summary>
        /// <param name="storageDirectory">The directory that will contain the files for the current set of tests.</param>
        /// <param name="hostId">The endpoint ID of the host application.</param>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer(string storageDirectory, EndpointId hostId)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UtilitiesModule());
                builder.RegisterModule(
                    new CommunicationModule(
                        new List<CommunicationSubject>
                            {
                                CommunicationSubjects.TestExecution,
                            },
                        new List<ChannelType>
                            {
                                ChannelType.NamedPipe,
                            }, 
                        false));

                builder.Register(
                        c =>
                        {
                            var ctx = c.Resolve<IComponentContext>();
                            return BuildReportFileUploader(ctx, storageDirectory, hostId);
                        })
                    .SingleInstance();

                RegisterFileSystem(builder);
                RegisterCore(builder, storageDirectory);
                RegisterCommands(builder);
                RegisterNotifications(builder);
                RegisterTestStepProcessors(builder);
            }

            return builder.Build();
        }
    }
}
