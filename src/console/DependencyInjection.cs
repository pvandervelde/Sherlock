//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Autofac;
using Autofac.Features.Metadata;
using Sherlock.Console.Nuclei;
using Sherlock.Shared.Core;

namespace Sherlock.Console
{
    /// <summary>
    /// Creates the dependency injection container with all the required references.
    /// </summary>
    internal static class DependencyInjection
    {
        private static void RegisterConfigurationReaderSelector(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        Func<Version, IVersionedConfigurationReader> readerSelector =
                            version =>
                            {
                                // Get a collection of lazy resolved meta data objects for the
                                // IVersionedXmlConfigurationReader
                                var allReadersAsLazy = ctx.Resolve<IEnumerable<Meta<IVersionedConfigurationReader>>>();

                                // Now find the reader that we want
                                // This is done by comparing the version numbers. If the
                                // first 2 digits of the input version number match the
                                // first 2 digits of the version number stored in the
                                // meta data, then we assume we found our reader.
                                // This is based on the idea that if we change the
                                // XML config format then we have to increment at least
                                // the minor version number.
                                IVersionedConfigurationReader selectedReader = null;
                                foreach (var reader in allReadersAsLazy)
                                {
                                    var inputShort = new Version(version.Major, version.Minor);

                                    var storedVersion = reader.Metadata["ReaderVersion"] as Version;
                                    var storedShort = new Version(storedVersion.Major, storedVersion.Minor);

                                    if (storedShort.Equals(inputShort))
                                    {
                                        selectedReader = reader.Value;
                                    }
                                }

                                return selectedReader;
                            };

                        return readerSelector;
                    });
        }

        private static void RegisterConfigurationReaders(ContainerBuilder builder)
        {
            builder.Register(c => new ConfigurationReaderVersion10(
                    c.Resolve<IFileSystem>(),
                    c.Resolve<StoreFileDataForEnvironment>()))
                .As<IVersionedConfigurationReader>()
                .WithMetadata<IReaderVersionMetaData>(
                    m => m.For(reader => reader.ReaderVersion, ConfigurationReaderVersion10.VersionToRead));

            builder.Register(c => new ConfigurationReaderVersion11(
                    c.Resolve<IFileSystem>(),
                    c.Resolve<StoreFileDataForEnvironment>()))
                .As<IVersionedConfigurationReader>()
                .WithMetadata<IReaderVersionMetaData>(
                    m => m.For(reader => reader.ReaderVersion, ConfigurationReaderVersion11.VersionToRead));
        }

        private static void RegisterFileSystem(ContainerBuilder builder)
        {
            builder.Register(c => new FileSystem())
                .As<IFileSystem>()
                .SingleInstance();
        }

        private static void RegisterTestFilePacker(ContainerBuilder builder)
        {
            builder.Register(c => new TestSuitePackage())
                .As<ITestSuitePackage>()
                .SingleInstance();

            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    StoreFileDataForEnvironment func =
                        (environment, stepOrder, storedPath, filePath) =>
                        {
                            var suitePackage = ctx.Resolve<ITestSuitePackage>();
                            var environmentPackage = suitePackage.Environment(environment);
                            if (environmentPackage == null)
                            {
                                environmentPackage = new TestEnvironmentPackage(environment);
                                suitePackage.Add(environmentPackage);
                            }

                            var testPackage = environmentPackage.Test(stepOrder);
                            if (testPackage == null)
                            {
                                testPackage = new TestStepPackage(stepOrder);
                                environmentPackage.Add(testPackage);
                            }

                            testPackage.Add(filePath, storedPath);
                        };

                    return func;
                });
        }

        /// <summary>
        /// Creates the DI container for the application.
        /// </summary>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UtilitiesModule());

                RegisterConfigurationReaderSelector(builder);
                RegisterConfigurationReaders(builder);
                RegisterFileSystem(builder);
                RegisterTestFilePacker(builder);
            }

            return builder.Build();
        }
    }
}
