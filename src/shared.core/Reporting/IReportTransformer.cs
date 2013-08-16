//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Sherlock.Shared.Core.Reporting
{
   /// <summary>
   /// Transforms an <see cref="IReport"/> object into a file based
   /// report.
   /// </summary>
   public interface IReportTransformer
   {
      /// <summary>
      /// Gets the file extension for the transformed report.
      /// </summary>
      string Extension
      {
         get;
      }

      /// <summary>
      /// Transforms the specified report into a pre-determined format and
      /// then stores the output in the stream.
      /// </summary>
      /// <param name="report">The report.</param>
      /// <param name="fileWriter">
      ///   The delegate that is used to write the given stream to a file
      ///   with the specified name.
      /// </param>
      void Transform(IReport report, Action<string, Stream> fileWriter);
   }
}
