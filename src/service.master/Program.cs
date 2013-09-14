//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;
using Autofac;
using Nuclei.Configuration;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Master.Nuclei.ExceptionHandling;
using Sherlock.Shared.Core;

namespace Sherlock.Service.Master
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "service.master.error.log";

        /// <summary>
        /// The DI container.
        /// </summary>
        private static IContainer s_Container;

        [STAThread]
        static int Main()
        {
            int functionReturnResult = -1;

            // var eventLogSource = Assembly.GetExecutingAssembly().GetName().Name;
            var result = TopLevelExceptionGuard.RunGuarded(
                () => functionReturnResult = RunApplication(),
                new IExceptionProcessor[]
                    {
                        new LogBasedExceptionProcessor(
                            LoggerBuilder.ForFile(
                                Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultErrorFileName),
                                new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now))), 
                    });

            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }

        private static int RunApplication()
        {
            var context = new ApplicationContext();
            s_Container = DependencyInjection.CreateContainer(context);
            var testCycle = s_Container.Resolve<ICycleTestsFromRequestToCompletion>();
            testCycle.Start();

            // Start with the message processing loop and then we 
            // wait for it to either get terminated or until we kill ourselves.
            Application.Run(context);

            // We probably won't get here given that the application has no real way of shutting down.
            return NormalApplicationExitCode;
        }
    }
}
