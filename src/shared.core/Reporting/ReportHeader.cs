//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace Sherlock.Shared.Core.Reporting
{
    /// <summary>
    /// Stores information concerning the header of a test report.
    /// </summary>
    [Serializable]
    public sealed class ReportHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportHeader"/> class.
        /// </summary>
        /// <param name="start">The date and time at which the test was started.</param>
        /// <param name="end">The date and time at which the test was finished.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="productVersion">The version of the product.</param>
        /// <param name="owner">The name of the user that started the test.</param>
        /// <param name="testDescription">The description for the test.</param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="productName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="productName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="productVersion"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when <paramref name="owner"/> is an empty string.
        /// </exception>
        public ReportHeader(
            DateTimeOffset start,
            DateTimeOffset end,
            string productName,
            string productVersion,
            string owner,
            string testDescription)
        {
            {
                Lokad.Enforce.Argument(() => productName);
                Lokad.Enforce.Argument(() => productName, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => productVersion);

                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => owner, Lokad.Rules.StringIs.NotEmpty);
            }

            SherlockVersion = Assembly.GetExecutingAssembly().GetName().Version;

            StartTime = start;
            EndTime = end;
            ProductName = productName;
            ProductVersion = productVersion;
            HostName = Environment.MachineName;
            UserName = owner;
            Description = testDescription;
        }

        /// <summary>
        /// Gets the name of the host machine on which the test was started.
        /// </summary>
        public string HostName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the user who started the test.
        /// </summary>
        public string UserName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version of the Sherlock application that was used to run the test.
        /// </summary>
        public Version SherlockVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time at which the test was started.
        /// </summary>
        public DateTimeOffset StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time at which the test was finished.
        /// </summary>
        public DateTimeOffset EndTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the product that was tested.
        /// </summary>
        public string ProductName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version of the product that was tested.
        /// </summary>
        public string ProductVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of the test.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
    }
}
