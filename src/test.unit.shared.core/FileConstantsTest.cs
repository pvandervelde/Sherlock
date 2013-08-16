//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace Sherlock.Shared.Core
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class FileConstantsTest
    {
        [Test]
        public void ProductSettingsPath()
        {
            var applicationConstants = new ApplicationConstants();
            var constants = new FileConstants(applicationConstants);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationConstants.CompanyName);
            path = Path.Combine(path, applicationConstants.ApplicationName);
            path = Path.Combine(path, applicationConstants.ApplicationCompatibilityVersion.ToString(2));

            Assert.AreEqual(path, constants.ProductSettingsUserPath());
        }

        [Test]
        public void LogPath()
        {
            var applicationConstants = new ApplicationConstants();
            var constants = new FileConstants(applicationConstants);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationConstants.CompanyName);
            path = Path.Combine(path, applicationConstants.ApplicationName);
            path = Path.Combine(path, applicationConstants.ApplicationCompatibilityVersion.ToString(2));
            path = Path.Combine(path, "logs");

            Assert.AreEqual(path, constants.LogPath());
        }
    }
}
