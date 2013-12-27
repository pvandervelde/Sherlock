//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ReportHeaderTest
    {
        [Test]
        public void RoundTripSerialise()
        {
            var productName = "product";
            var productVersion = "1.2.3.4";
            var owner = "owner";
            var description = "description";
            var start = DateTimeOffset.Now;
            var end = start.AddSeconds(10);

            var header = new ReportHeader(
                start,
                end,
                productName,
                productVersion,
                owner,
                description);
            var otherHeader = AssertExtensions.RoundTripSerialize(header);

            Assert.AreEqual(header.StartTime, otherHeader.StartTime);
            Assert.AreEqual(header.EndTime, otherHeader.EndTime);

            Assert.AreEqual(header.HostName, otherHeader.HostName);
            Assert.AreEqual(header.UserName, otherHeader.UserName);
            Assert.AreEqual(header.SherlockVersion, otherHeader.SherlockVersion);

            Assert.AreEqual(header.ProductName, otherHeader.ProductName);
            Assert.AreEqual(header.ProductVersion, otherHeader.ProductVersion);
        }

        [Test]
        public void CreateWithEmptyProductName()
        {
            Assert.Throws<ArgumentException>(
                () => new ReportHeader(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now,
                    string.Empty,
                    "1.1",
                    "owner",
                    "description"));
        }

        [Test]
        public void CreateWithEmptyOwner()
        {
            Assert.Throws<ArgumentException>(
                () => new ReportHeader(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now,
                    "Product",
                    "1.1",
                    string.Empty,
                    "description"));
        }

        [Test]
        public void Create()
        {
            var productName = "product";
            var productVersion = "1.2.3.4";
            var owner = "owner";
            var description = "description";
            var start = DateTimeOffset.Now;
            var end = start.AddSeconds(10);

            var header = new ReportHeader(
                start,
                end,
                productName,
                productVersion,
                owner,
                description);

            var sherlockVersion = typeof(ReportHeader).Assembly.GetName().Version;

            Assert.AreEqual(start, header.StartTime);
            Assert.AreEqual(end, header.EndTime);

            Assert.AreEqual(Environment.MachineName, header.HostName);
            Assert.AreEqual(owner, header.UserName);
            Assert.AreEqual(sherlockVersion, header.SherlockVersion);

            Assert.AreEqual(productName, header.ProductName);
            Assert.AreEqual(productVersion, header.ProductVersion);
        }
    }
}
