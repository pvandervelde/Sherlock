//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Tracing;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Sherlock.Web.Api
{
    /// <summary>
    /// Implements the <see cref="ITraceWriter"/> interface and writes all the traces to 
    /// the <see cref="SystemDiagnostics"/> object.
    /// </summary>
    internal sealed class NucleiBasedTraceWriter : ITraceWriter
    {
        private static LevelToLog FromTraceLevelToLogLevel(TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Off:
                    return LevelToLog.None;
                case TraceLevel.Debug:
                    return LevelToLog.Debug;
                case TraceLevel.Info:
                    return LevelToLog.Info;
                case TraceLevel.Warn:
                    return LevelToLog.Warn;
                case TraceLevel.Error:
                    return LevelToLog.Error;
                case TraceLevel.Fatal:
                    return LevelToLog.Fatal;
                default:
                    throw new ArgumentOutOfRangeException("traceLevel");
            }
        }

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="NucleiBasedTraceWriter"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public NucleiBasedTraceWriter(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Invokes the specified traceAction to allow setting values in a new <see cref="T:System.Web.Http.Tracing.TraceRecord"/> if 
        /// and only if tracing is permitted at the given category and level. 
        /// </summary>
        /// <param name="request">
        /// The current <see cref="T:System.Net.Http.HttpRequestMessage"/>. It may be null but doing so will prevent subsequent trace 
        /// analysis from correlating the trace to a particular request.
        /// </param>
        /// <param name="category">The logical category for the trace. Users can define their own.</param>
        /// <param name="level">The <see cref="T:System.Web.Http.Tracing.TraceLevel"/> at which to write this trace.</param>
        /// <param name="traceAction">
        /// The action to invoke if tracing is enabled.  The caller is expected to fill in the fields of the given 
        /// <see cref="T:System.Web.Http.Tracing.TraceRecord"/> in this action.
        /// </param>
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level != TraceLevel.Off)
            {
                var record = new TraceRecord(request, category, level);
                traceAction(record);

                var message = new StringBuilder();
                if (record.Request != null)
                {
                    if (record.Request.Method != null)
                    {
                        message.Append(record.Request.Method);
                    }

                    if (record.Request.RequestUri != null)
                    {
                        message.Append(" ").Append(record.Request.RequestUri);
                    }
                }

                if (!string.IsNullOrWhiteSpace(record.Category))
                {
                    message.Append(" ").Append(record.Category);
                }

                if (!string.IsNullOrWhiteSpace(record.Operator))
                {
                    message.Append(" ").Append(record.Operator).Append(" ").Append(record.Operation);
                }

                if (!string.IsNullOrWhiteSpace(record.Message))
                {
                    message.Append(" ").Append(record.Message);
                }

                if (record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
                {
                    message.Append(" ").Append(record.Exception.GetBaseException().Message);
                }

                m_Diagnostics.Log(
                    FromTraceLevelToLogLevel(record.Level),
                    WebApiConstants.LogPrefix,
                    message.ToString());
            }
        }
    }
}
