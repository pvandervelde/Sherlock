﻿//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Autofac;
using Mono.Options;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Console.Nuclei.ExceptionHandling;
using Sherlock.Console.Properties;
using Sherlock.Shared.Core;

namespace Sherlock.Console
{
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
        private const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The exit code used when the application was not able to gather all the required data
        /// from the application configuration file.
        /// </summary>
        private const int InvalidApplicationConfigurationExitCode = 2;

        /// <summary>
        /// The exit code used when the application has been provided with one or more invalid
        /// command line parameters.
        /// </summary>
        private const int InvalidCommandLineParametersExitCode = 3;

        /// <summary>
        /// The exit code used when the application has been provided with the path to a 
        /// configuration file that does not exist.
        /// </summary>
        private const int InvalidConfigurationFileExitCode = 4;

        /// <summary>
        /// The exit code used when the application was not able to load the test information 
        /// from the provided configuration file.
        /// </summary>
        private const int CouldNotLoadTestInformationExitCode = 5;

        /// <summary>
        /// The exit code used when the application was not able to register the test.
        /// </summary>
        private const int TestRegistrationFailedExitCode = 6;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "console.error.log";

        /// <summary>
        /// The dependency injection container.
        /// </summary>
        private static IContainer s_Container;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private static SystemDiagnostics s_Diagnostics;

        /// <summary>
        /// The object that contains the configuration for the application.
        /// </summary>
        private static IConfiguration s_Configuration;

        /// <summary>
        /// A flag indicating if the help information for the application should be displayed.
        /// </summary>
        private static bool s_ShouldDisplayHelp;

        /// <summary>
        /// The full path to the configuration file that contains the test description.
        /// </summary>
        private static string s_ConfigurationFile;

        [STAThread]
        static int Main(string[] args)
        {
            int functionReturnResult = -1;

            var processor = new LogBasedExceptionProcessor(
                LoggerBuilder.ForFile(
                    Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultErrorFileName),
                    new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now)));
            var result = TopLevelExceptionGuard.RunGuarded(
                () => functionReturnResult = RunApplication(args),
                new ExceptionProcessor[]
                    {
                        processor.Process
                    });

            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're just catching and then exiting the app.")]
        private static int RunApplication(IEnumerable<string> args)
        {
            try
            {
                ShowHeader();
                LoadKernel();
                LogStartup();

                // Parse the command line options
                var options = CreateOptionSet();
                try
                {
                    options.Parse(args);
                }
                catch (OptionException e)
                {
                    s_Diagnostics.Log(
                            LevelToLog.Fatal,
                            ConsoleConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Error_InvalidInputParameters_WithException,
                                e));

                    WriteErrorToConsole(Resources.Output_Error_InvalidInput);
                    return InvalidCommandLineParametersExitCode;
                }

                if (s_ShouldDisplayHelp)
                {
                    ShowHelp(options);
                    return HelpShownExitCode;
                }

                if (string.IsNullOrWhiteSpace(s_ConfigurationFile))
                {
                    s_Diagnostics.Log(LevelToLog.Fatal, ConsoleConstants.LogPrefix, Resources.Output_Error_MissingValues);
                    WriteErrorToConsole(Resources.Output_Error_MissingValues);
                    ShowHelp(options);
                    return InvalidCommandLineParametersExitCode;
                }

                if (!File.Exists(s_ConfigurationFile))
                {
                    s_Diagnostics.Log(
                        LevelToLog.Fatal,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_ConfigurationFileDoesNotExist,
                            s_ConfigurationFile));

                    WriteErrorToConsole(Resources.Output_Error_ConfigurationFileDoesNotExist);
                    return InvalidConfigurationFileExitCode;
                }

                if (!s_Configuration.HasValueFor(ConsoleConfigurationKeys.WebServiceUrl))
                {
                    s_Diagnostics.Log(
                        LevelToLog.Fatal,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_MissingApplicationConfigurationValue,
                            ConsoleConfigurationKeys.WebServiceUrl));

                    WriteErrorToConsole(Resources.Output_Error_MissingWebServiceUrlConfiguration);
                    return InvalidApplicationConfigurationExitCode;
                }
                
                var serverUrl = new Uri(s_Configuration.Value<string>(ConsoleConfigurationKeys.WebServiceUrl));

                WriteInputParametersToLog(s_ConfigurationFile);
                return QueueTest(serverUrl);
            }
            finally
            {
                if (s_Container != null)
                {
                    s_Container.Dispose();
                }
            }
        }

        private static void ShowHeader()
        {
            System.Console.WriteLine(Resources.Header_ApplicationAndVersion, GetVersion());
            System.Console.WriteLine(GetCopyright());
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
            try
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(errorText);
            }
            finally
            {
                System.Console.ResetColor();
            }
        }

        private static void WriteToConsole(string text)
        {
            System.Console.WriteLine(text);
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
                @"Sherlock.Console.Properties.licenses.xml");
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

        private static void LoadKernel()
        {
            s_Container = DependencyInjection.CreateContainer();
            s_Diagnostics = s_Container.Resolve<SystemDiagnostics>();
            s_Configuration = s_Container.Resolve<IConfiguration>();
        }

        private static void LogStartup()
        {
            s_Diagnostics.Log(
                LevelToLog.Info,
                ConsoleConstants.LogPrefix,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Log_Information_ApplicationAndVersion,
                    GetVersion()));
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
                        Resources.CommandLine_Param_ConfigurationFile_Key,
                        Resources.CommandLine_Param_ConfigurationFile_Description,
                        v => s_ConfigurationFile = v
                    },
                };
            return options;
        }

        private static void WriteInputParametersToLog(string configurationFile)
        {
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Information_InputParameterConfigurationFile,
                    configurationFile));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Once we catch the exception we'll exit, but now we should be able to log the failure.")]
        private static int QueueTest(Uri serverUrl)
        {
            try
            {
                WriteToConsole(Resources.Output_Information_ReadingConfigurationFile);
                s_Diagnostics.Log(
                    LevelToLog.Info, 
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Information_ReadingConfigurationFile_WithFile,
                        s_ConfigurationFile));

                var configurationInfo = LoadTestCaseFromConfiguration(s_ConfigurationFile);
                if (configurationInfo == null)
                {
                    return CouldNotLoadTestInformationExitCode;
                }

                var storageDirectory = CreateLocalStorageDirectory();
                var zipFile = Path.Combine(
                    storageDirectory,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}-{1}.zip",
                        configurationInfo.Test.ProductUnderTest,
                        configurationInfo.Test.VersionOfProductUnderTest));
                try
                {
                    var packer = s_Container.Resolve<ITestSuitePackage>();
                    packer.PackTo(zipFile);

                    var http = new HttpClient();
                    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    RegisterTest(http, serverUrl, configurationInfo.Test);
                    RegisterTestEnvironments(http, serverUrl, configurationInfo.Test.Id, configurationInfo.Test.Environments);
                    var environmentMapping = MapEnvironmentsToTestSteps(configurationInfo.Test);

                    RegisterTestSteps(http, serverUrl, configurationInfo.Test.TestSteps, environmentMapping);
                    RegisterTestStepParameters(http, serverUrl, configurationInfo.Test.TestSteps);
                    RegisterTestStepFilesToTransferBack(http, serverUrl, configurationInfo.Test.TestSteps);
                    UploadTestFiles(http, serverUrl, configurationInfo.Test.Id, zipFile);
                    MarkTestAsReady(http, serverUrl, configurationInfo.Test.Id);

                    s_Diagnostics.Log(
                        LevelToLog.Info,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Information_TestQueued,
                            configurationInfo.Test.Id));
                    WriteToConsole(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Output_Information_TestQueued_WithId,
                            configurationInfo.Test.Id));

                    return NormalApplicationExitCode;
                }
                finally
                {
                    if (File.Exists(zipFile))
                    {
                        try
                        {
                            File.Delete(zipFile);
                        }
                        catch (IOException)
                        {
                            // Do nothing ...
                        }
                    }
                }
            }
            catch (InvalidServerResponseException e)
            {
                s_Diagnostics.Log(
                    LevelToLog.Fatal,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Error_ProcessingError_WithException,
                        e));

                WriteErrorToConsole(Resources.Output_Error_WhileProcessing);
                return TestRegistrationFailedExitCode;
            }
            catch (Exception e)
            {
                s_Diagnostics.Log(
                    LevelToLog.Fatal,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Error_ProcessingError_WithException,
                        e));

                WriteErrorToConsole(Resources.Output_Error_WhileProcessing);
                return UnhandledExceptionApplicationExitCode;
            }
        }

        private static ConfigurationInfo LoadTestCaseFromConfiguration(string configurationFile)
        {
            var doc = XDocument.Load(configurationFile);

            var configurationVersion = (from item in doc.Elements("sherlock")
                                        select new Version(item.Attribute("configurationVersion").Value)).First();

            var func = s_Container.Resolve<Func<Version, IVersionedConfigurationReader>>();
            var reader = func(configurationVersion);
            if (reader == null)
            {
                s_Diagnostics.Log(
                    LevelToLog.Error,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Error_ConfigurationFileVersionUnknown_WithVersion,
                        configurationVersion));

                throw new NonMatchingVersionFoundException();
            }

            try
            {
                return reader.Read(doc);
            }
            catch (InvalidConfigurationFileException e)
            {
                s_Diagnostics.Log(
                    LevelToLog.Error, 
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Error_FailedToReadConfigurationFile_WithError,
                        e));

                WriteErrorToConsole(e.Message);
                throw;
            }
        }

        private static string CreateLocalStorageDirectory()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            Debug.Assert(attributes.Length == 1, "There should only be 1 AssemblyProductAttribute");

            var product = ((AssemblyProductAttribute)attributes[0]).Product;
            var path = Path.Combine(
                Path.GetTempPath(),
                product,
                Guid.NewGuid().ToString("D"));

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (IOException e)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Error,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_FailedToCreateStorageDirectory_WithDirectoryAndError,
                            path,
                            e));
                }
            }

            return path;
        }

        private static void RegisterTest(HttpClient http, Uri serverUrl, TestDescription test)
        {
            WriteToConsole(Resources.Output_Information_RegisteringTest);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_RegisteringTest);

            var registerUrl = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/api/Test/Register?product={1}&version={2}&owner={3}&description={4}&reportpath={5}",
                serverUrl.AbsoluteUri,
                Uri.EscapeDataString(test.ProductUnderTest),
                Uri.EscapeDataString(test.VersionOfProductUnderTest),
                Uri.EscapeDataString(test.Owner),
                Uri.EscapeDataString(test.Description),
                Uri.EscapeDataString(test.ReportPath));

            var response = MakePostRequest(http, new Uri(registerUrl), null);
            if (!response.IsSuccessStatusCode)
            {
                s_Diagnostics.Log(
                    LevelToLog.Error,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Error_TestRegistrationFailed_WithStatusCodeAndReason,
                        response.StatusCode,
                        response.ReasonPhrase));
                WriteErrorToConsole(Resources.Output_Error_TestRegistrationFailed);

                throw new InvalidServerResponseException();
            }

            var storeTask = GetIdFromReponse(response);
            test.Id = storeTask.Result;

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Log_Trace_TestRegistrationComplete_WithTestId,
                    test.Id));
        }

        private static Task<int> GetIdFromReponse(HttpResponseMessage response)
        {
            var readTask = response.Content.ReadAsStringAsync();
            var storeTask = readTask.ContinueWith(
                t =>
                {
                    var xDoc = XDocument.Parse(t.Result);
                    var idText = (xDoc.FirstNode as XElement).Value;
                    var id = int.Parse(idText, CultureInfo.InvariantCulture);

                    return id;
                });

            return storeTask;
        }

        private static void RegisterTestEnvironments(HttpClient http, Uri serverUrl, int testId, IEnumerable<TestEnvironmentDescription> environments)
        {
            WriteToConsole(Resources.Output_Information_RegisteringTestEnvironments);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_RegisteringTestEnvironments);

            var tasks = new List<Task>();
            foreach (var environment in environments)
            {
                var localEnvironment = environment;
                s_Diagnostics.Log(
                    LevelToLog.Trace,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Trace_RegisteringTestEnvironment_WithEnvironment,
                        localEnvironment.Name,
                        localEnvironment.OperatingSystem));

                var registerUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/api/TestEnvironment/Register?name={1}&test={2}",
                    serverUrl.AbsoluteUri,
                    Uri.EscapeDataString(localEnvironment.Name),
                    testId);

                string xmlText;
                using (var wr = new StringWriter())
                {
                    var xmlDoc = GenerateEnvironmentDataStorage(localEnvironment.OperatingSystem, localEnvironment.Applications);
                    xmlDoc.Save(wr);
                    
                    wr.Flush();
                    xmlText = wr.ToString();
                }

                var content = new StringContent(xmlText, Encoding.UTF8, "application/xml");
                var response = MakePostRequest(http, new Uri(registerUrl), content);
                if (!response.IsSuccessStatusCode)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Error,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_TestEnvironmentRegistrationFailed_WithStatusCodeAndReason,
                            response.StatusCode,
                            response.ReasonPhrase));
                    WriteErrorToConsole(Resources.Output_Error_TestEnvironmentRegistrationFailed);

                    throw new InvalidServerResponseException();
                }

                var task = GetIdFromReponse(response);
                var storeTask = task.ContinueWith(
                    t =>
                    {
                        localEnvironment.Id = t.Result;
                    });

                tasks.Add(storeTask);
            }

            Task.WaitAll(tasks.ToArray());

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_TestEnvironmentsRegistrationComplete);
        }

        private static XmlDocument GenerateEnvironmentDataStorage(
            OperatingSystemDescription operatingSystemDescription, 
            IEnumerable<ApplicationDescription> applications)
        {
            var result = new XmlDocument();
            var rootNode = result.CreateElement("environment");
            result.AppendChild(rootNode);

            var operatingSystemNode = result.CreateElement("operatingsystem");
            rootNode.AppendChild(operatingSystemNode);

            // store the operating system information in the same way as was stored in
            // the original config file.
            {
                var nameAttribute = result.CreateAttribute("name");
                nameAttribute.Value = operatingSystemDescription.Name;
                operatingSystemNode.Attributes.Append(nameAttribute);

                var servicePackAttribute = result.CreateAttribute("servicepack");
                servicePackAttribute.Value = operatingSystemDescription.ServicePack;
                operatingSystemNode.Attributes.Append(servicePackAttribute);

                var cultureAttribute = result.CreateAttribute("culture");
                cultureAttribute.Value = operatingSystemDescription.Culture.Name;
                operatingSystemNode.Attributes.Append(cultureAttribute);

                var pointerSizeAttribute = result.CreateAttribute("architecturepointersize");
                pointerSizeAttribute.Value = operatingSystemDescription.PointerSize.ToString(CultureInfo.InvariantCulture);
                operatingSystemNode.Attributes.Append(pointerSizeAttribute);
            }

            var applicationsNode = result.CreateElement("applications");
            rootNode.AppendChild(applicationsNode);
            foreach (var application in applications)
            {
                var applicationNode = result.CreateElement("application");
                applicationsNode.AppendChild(applicationNode);

                var nameAttribute = result.CreateAttribute("name");
                nameAttribute.Value = application.Name;
                applicationNode.Attributes.Append(nameAttribute);

                var versionAttribute = result.CreateAttribute("version");
                versionAttribute.Value = application.Version.ToString(4);
                applicationNode.Attributes.Append(versionAttribute);
            }

            return result;
        }

        private static IDictionary<string, int> MapEnvironmentsToTestSteps(TestDescription testCase)
        {
            return testCase.Environments.ToDictionary(s => s.Name, s => s.Id);
        }

        private static void RegisterTestSteps(
            HttpClient http, 
            Uri serverUrl, 
            IEnumerable<TestStepDescription> testSteps, 
            IDictionary<string, int> environmentToIdMapping)
        {
            WriteToConsole(Resources.Output_Information_RegisteringTestSteps);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_RegisteringTestSteps);

            var tasks = new List<Task>();
            foreach (var step in testSteps)
            {
                var localStep = step;
                s_Diagnostics.Log(
                    LevelToLog.Trace,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Trace_RegisteringTestStep_WithTestStep,
                        localStep.Order,
                        localStep.GetType()));

                var registerUrl = CreateTestStepRegisterUrl(serverUrl, localStep, environmentToIdMapping);
                var response = MakePostRequest(http, new Uri(registerUrl), null);
                if (!response.IsSuccessStatusCode)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Error,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_TestStepRegistrationFailed_WithStatusCodeAndReason,
                            response.StatusCode,
                            response.ReasonPhrase));
                    WriteErrorToConsole(Resources.Output_Error_TestStepRegistrationFailed);

                    throw new InvalidServerResponseException();
                }

                var task = GetIdFromReponse(response);
                var storeTask = task.ContinueWith(
                    t =>
                    {
                        localStep.Id = t.Result;
                    });

                tasks.Add(storeTask);
            }

            Task.WaitAll(tasks.ToArray());

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_TestStepRegistrationComplete);
        }

        private static string CreateTestStepRegisterUrl(Uri serverUrl, TestStepDescription step, IDictionary<string, int> environmentToIdMapping)
        {
            string stepType = string.Empty;
            string parameters = string.Empty;

            var consoleStep = step as ConsoleExecuteTestStepDescription;
            if (consoleStep != null)
            {
                stepType = "Console";
                parameters = string.Format(
                    CultureInfo.InvariantCulture,
                    "&executablePath={0}",
                    Uri.EscapeDataString(consoleStep.ExecutablePath));
            }

            var msiStep = step as MsiInstallTestStepDescription;
            if (msiStep != null)
            {
                stepType = "Msi";
            }

            var scriptStep = step as ScriptExecuteTestStepDescription;
            if (scriptStep != null)
            {
                stepType = "Script";
                parameters = string.Format(
                    CultureInfo.InvariantCulture,
                    "&language={0}",
                    Uri.EscapeDataString(scriptStep.ScriptLanguage));
            }

            var xcopyStep = step as XCopyTestStepDescription;
            if (xcopyStep != null)
            {
                stepType = "XCopy";
                parameters = string.Format(
                    CultureInfo.InvariantCulture,
                    "&destination={0}",
                    Uri.EscapeDataString(xcopyStep.Destination));
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}/api/TestStep/Register{1}?environment={2}&order={3}&failuremode={4}&shouldincludesystemlog={5}{6}",
                serverUrl.AbsoluteUri,
                stepType,
                environmentToIdMapping[step.Environment],
                step.Order,
                step.FailureMode,
                step.IncludeSystemLogFileInReport,
                parameters);
        }

        private static void RegisterTestStepParameters(HttpClient http, Uri serverUrl, IEnumerable<TestStepDescription> testSteps)
        {
            WriteToConsole(Resources.Output_Information_RegisteringTestStepParameters);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_RegisteringTestStepParameters);

            foreach (var step in testSteps)
            {
                foreach (var parameter in step.Parameters)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Trace,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Trace_RegisteringTestStepParameter_WithParameter,
                            parameter.Key,
                            parameter.Value));

                    var registerUrl = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/api/TestStepParameter/Register?teststep={1}&key={2}&value={3}",
                        serverUrl.AbsoluteUri,
                        step.Id,
                        Uri.EscapeDataString(parameter.Key),
                        Uri.EscapeDataString(parameter.Value));
                    var response = MakePostRequest(http, new Uri(registerUrl), null);
                    if (!response.IsSuccessStatusCode)
                    {
                        s_Diagnostics.Log(
                            LevelToLog.Error,
                            ConsoleConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Error_TestStepParameterRegistrationFailed_WithStatusCodeAndReason,
                                response.StatusCode,
                                response.ReasonPhrase));
                        WriteErrorToConsole(Resources.Output_Error_TestStepParameterRegistrationFailed);

                        throw new InvalidServerResponseException();
                    }
                }
            }

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_TestStepParametersRegistrationCompleted);
        }

        private static void RegisterTestStepFilesToTransferBack(
            HttpClient http, 
            Uri serverUrl, 
            IEnumerable<TestStepDescription> testSteps)
        {
            WriteToConsole(Resources.Output_Information_RegisteringTestStepAdditionalReportFiles);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_RegisteringTestStepAdditionalReportFiles);

            foreach (var step in testSteps)
            {
                foreach (var fileElement in step.FileElementsToIncludeInReport)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Trace,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Trace_RegisteringTestStepAdditionalReportFiles_WithPath,
                            fileElement.FullName));

                    string registerUrl = null;

                    var file = fileElement as FileInfo;
                    if (file != null)
                    {
                        registerUrl = string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}/api/TestStepAdditionalReportFiles/ForFile?teststep={1}&path={2}",
                            serverUrl.AbsoluteUri,
                            step.Id,
                            Uri.EscapeDataString(file.FullName));
                    }

                    var directory = fileElement as DirectoryInfo;
                    if (directory != null)
                    {
                        registerUrl = string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}/api/TestStepAdditionalReportFiles/ForDirectory?teststep={1}&path={2}",
                            serverUrl.AbsoluteUri,
                            step.Id,
                            Uri.EscapeDataString(directory.FullName));
                    }

                    if (string.IsNullOrWhiteSpace(registerUrl))
                    {
                        continue;
                    }

                    var response = MakePostRequest(http, new Uri(registerUrl), null);
                    if (!response.IsSuccessStatusCode)
                    {
                        s_Diagnostics.Log(
                            LevelToLog.Error,
                            ConsoleConstants.LogPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Error_TestStepParameterRegistrationFailed_WithStatusCodeAndReason,
                                response.StatusCode,
                                response.ReasonPhrase));
                        WriteErrorToConsole(Resources.Output_Error_TestStepParameterRegistrationFailed);

                        throw new InvalidServerResponseException();
                    }
                }
            }

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_TestStepAdditionalReportFilesRegistrationCompleted);
        }

        private static void UploadTestFiles(HttpClient http, Uri serverUrl, int testId, string file)
        {
            WriteToConsole(Resources.Output_Information_UploadingTestFiles);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_UploadingTestFiles);

            var uploadUrl = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/api/Test/Upload/{1}",
                serverUrl.AbsoluteUri,
                testId);

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                s_Diagnostics.Log(
                    LevelToLog.Trace,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Trace_UploadingTestFile_WithFile,
                        file));

                var content = new StreamContent(fileStream);
                var response = MakePostRequest(http, new Uri(uploadUrl), content);
                if (!response.IsSuccessStatusCode)
                {
                    s_Diagnostics.Log(
                        LevelToLog.Error,
                        ConsoleConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Error_TestFileUploadFailed_WithStatusCodeAndReason,
                            response.StatusCode,
                            response.ReasonPhrase));
                    WriteErrorToConsole(Resources.Output_Error_TestFileUploadFailed);

                    throw new InvalidServerResponseException();
                }
            }

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_TestFilesUploadCompleted);
        }

        private static void MarkTestAsReady(HttpClient http, Uri serverUrl, int testId)
        {
            WriteToConsole(Resources.Output_Information_MarkingTestAsReady);
            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_MarkingTestAsReady);

            var markAsReadyUrl = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/api/Test/MarkAsReady/{1}",
                serverUrl.AbsoluteUri,
                testId);

            var response = MakePostRequest(http, new Uri(markAsReadyUrl), null);
            if (!response.IsSuccessStatusCode)
            {
                s_Diagnostics.Log(
                    LevelToLog.Error,
                    ConsoleConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Error_MarkingTestAsReadyFailed_WithStatusCodeAndReason,
                        response.StatusCode,
                        response.ReasonPhrase));
                WriteErrorToConsole(Resources.Output_Error_MarkingTestAsReadyFailed);

                throw new InvalidServerResponseException();
            }

            s_Diagnostics.Log(
                LevelToLog.Trace,
                ConsoleConstants.LogPrefix,
                Resources.Log_Trace_TestMarkedAsReady);
        }

        private static HttpResponseMessage MakePostRequest(HttpClient http, Uri url, HttpContent data)
        {
            var httpResponse = http.PostAsync(url, data).Result;
            return httpResponse;
        }
    }
}
