//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using Lokad;
using Lokad.Rules;

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
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="productName"/> is <see langword="null" />.
      /// </exception>
      /// <exception cref="ArgumentException">
      ///   Thrown when <paramref name="productName"/> is an empty string.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      ///   Thrown when <paramref name="productVersion"/> is <see langword="null" />.
      /// </exception>
      public ReportHeader(
         DateTimeOffset start,
         DateTimeOffset end,
         string productName,
         string productVersion)
      {
         {
            Enforce.Argument(() => productName);
            Enforce.Argument(() => productName, StringIs.NotEmpty);

            Enforce.Argument(() => productVersion);
         }

         HostName = Environment.MachineName;
         UserName = string.Format(
            CultureInfo.InvariantCulture,
            @"{0}\{1}",
            Environment.UserDomainName,
            Environment.UserName);

         SherlockVersion = Assembly.GetExecutingAssembly().GetName().Version;

         StartTime = start;
         EndTime = end;
         ProductName = productName;
         ProductVersion = productVersion;
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
   }
}
