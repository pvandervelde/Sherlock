//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Mono.Options;
using Nuclei;
using Sherlock.Verification.Console.Properties;

namespace Sherlock.Verification.Console
{
    /// <summary>
    /// Defines the entry point for the verification console application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The goal for this application is to provide a simple way of passing or failing one
    /// or more test steps, so that a user of the Sherlock system can easily verify that
    /// all parts of the system are functional.
    /// </para>
    /// <para>
    /// In order to achieve this the application will output several lines of information in the console
    /// and then either exit with a zero exit code (success) or a non-zero exit code (fail). The user
    /// can specify which of the scenario's is desired through the command line options.
    /// </para>
    /// </remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// The exit code used when the application has shown the help information.
        /// </summary>
        private const int HelpShownExitCode = -1;

        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int FailApplicationExitCode = 1;

        /// <summary>
        /// A flag indicating if the help information for the application should be displayed.
        /// </summary>
        private static bool s_ShouldDisplayHelp;

        /// <summary>
        /// A flag indicating if the license information should be displayed.
        /// </summary>
        private static bool s_ShouldDisplayLicenses;

        /// <summary>
        /// A flag indicating if the application should exit with a failure code or not.
        /// </summary>
        private static bool s_ShouldFail;

        /// <summary>
        /// A flag indicating if the application should crash or not.
        /// </summary>
        private static bool s_ShouldCrash;

        /// <summary>
        /// The application entry point.
        /// </summary>
        /// <param name="args">The application input arguments.</param>
        /// <returns>An exit code indicating if the application executed successfully or not.</returns>
        static int Main(string[] args)
        {
            ShowHeader();

            // Parse the command line options
            var options = CreateOptionSet();
            try
            {
                options.Parse(args);
            }
            catch (OptionException)
            {
                return FailApplicationExitCode;
            }

            if (s_ShouldDisplayLicenses)
            {
                ShowLicenses();
            }

            if (s_ShouldDisplayHelp)
            {
                ShowHelp(options);
                return HelpShownExitCode;
            }

            if (s_ShouldCrash)
            {
                WriteErrorToConsole(Resources.Output_Error_AboutToExitApplicationWithCrash);
                throw new Exception();
            }

            if (s_ShouldFail)
            {
                WriteErrorToConsole(Resources.Output_Error_AboutToExitApplicationWithError);
                return FailApplicationExitCode;
            }

            return NormalApplicationExitCode;
        }

        private static void ShowHeader()
        {
            System.Console.WriteLine(Resources.Header_ApplicationAndVersion, GetVersion());
            System.Console.WriteLine(GetCopyright());
        }

        private static void ShowLicenses()
        {
            System.Console.WriteLine(GetLibraryLicenses());
        }

        private static void ShowHelp(OptionSet argProcessor)
        {
            System.Console.WriteLine(Resources.Help_Usage_Intro);
            System.Console.WriteLine();
            argProcessor.WriteOptionDescriptions(System.Console.Out);
        }

        private static void WriteErrorToConsole(string errorText)
        {
            System.Console.Error.WriteLine(errorText);
        }

        private static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            Debug.Assert(attribute.Length == 1, "There should be a copyright attribute.");

            return (attribute[0] as AssemblyFileVersionAttribute).Version;
        }

        private static string GetCopyright()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            Debug.Assert(attribute.Length == 1, "There should be a copyright attribute.");

            return (attribute[0] as AssemblyCopyrightAttribute).Copyright;
        }

        private static string GetLibraryLicenses()
        {
            var licenseXml = EmbeddedResourceExtracter.LoadEmbeddedStream(
                Assembly.GetExecutingAssembly(),
                @"Sherlock.Verification.Console.Properties.licenses.xml");
            var doc = XDocument.Load(licenseXml);
            var licenses = from element in doc.Descendants("package")
                           select new
                           {
                               Id = element.Element("id").Value,
                               Version = element.Element("version").Value,
                               Source = (element.Element("url").FirstNode as XCData).Value,
                               License = (element.Element("licenseurl").FirstNode as XCData).Value,
                           };

            var builder = new StringBuilder();
            builder.AppendLine(Resources.Header_OtherPackages_Intro);
            foreach (var license in licenses)
            {
                builder.AppendLine(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Header_OtherPackages_IdAndLicense,
                        license.Id,
                        license.Version,
                        license.Source));
            }

            return builder.ToString();
        }

        private static OptionSet CreateOptionSet()
        {
            var options = new OptionSet 
                {
                    { 
                        Resources.CommandLine_Options_Help_Key, 
                        Resources.CommandLine_Options_Help_Description, 
                        v => s_ShouldDisplayHelp = v != null
                    },
                    {
                        Resources.CommandLine_Param_DisplayLicenses_Key,
                        Resources.CommandLine_Param_DisplayLicenses_Description,
                        v => s_ShouldDisplayLicenses = v != null
                    },
                    {
                        Resources.CommandLine_Param_ShouldFail_Key,
                        Resources.CommandLine_Param_ShouldFail_Description,
                        v => s_ShouldFail = v != null
                    },
                    {
                        Resources.CommandLine_Param_ShouldCrash_Key,
                        Resources.CommandLine_Param_ShouldCrash_Description,
                        v => s_ShouldCrash = v != null
                    },
                };
            return options;
        }
    }
}
