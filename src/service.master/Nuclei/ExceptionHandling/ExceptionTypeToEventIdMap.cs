//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Sherlock.Service.Master.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines methods that map an <see cref="Exception"/> to an ID number for error reporting
    /// purposes.
    /// </summary>
    internal static class ExceptionTypeToEventIdMap
    {
        /// <summary>
        /// The table that maps an exception type to an event ID.
        /// </summary>
        private static readonly Dictionary<Type, int> s_ExceptionTypeToEventIdMap =
            new Dictionary<Type, int> 
                { 
                    // Runtime errors
                    { typeof(OutOfMemoryException), 0 },
                    { typeof(StackOverflowException), 1 },
                    { typeof(AccessViolationException), 2 },
                    
                    // Standard errors
                    { typeof(AppDomainUnloadedException), 50 },
                    { typeof(ArgumentException), 51 },
                    { typeof(ArgumentNullException), 52 },
                    { typeof(ArgumentOutOfRangeException), 53 },
                    
                    // I/O
                    { typeof(IOException), 100 },
                    { typeof(DirectoryNotFoundException), 101 },
                    { typeof(DriveNotFoundException), 102 },
                    { typeof(EndOfStreamException), 103 },
                    { typeof(FileLoadException), 104 },
                    { typeof(FileNotFoundException), 105 },
                    { typeof(InternalBufferOverflowException), 106 },
                    { typeof(InvalidDataException), 107 },
                    { typeof(PathTooLongException), 108 },

                    // Exception, used in case nothing else fits
                    //
                    // The maximum value of an EventID is defined as UInt16.MaxValue
                    // (i.e. 65535) here: http://msdn.microsoft.com/en-us/library/ysx54zda.aspx
                    { typeof(Exception), ushort.MaxValue }
                };

        /// <summary>
        /// Maps an <see cref="Exception"/> to an ID number for error reporting purposes.
        /// </summary>
        /// <param name="exception">The exception for which the ID number should be found.</param>
        /// <returns>
        /// The ID number for the error.
        /// </returns>
        public static int EventIdForException(Exception exception)
        {
            var exceptionType = exception.GetType();
            while (!s_ExceptionTypeToEventIdMap.ContainsKey(exceptionType))
            {
                exceptionType = exceptionType.BaseType;
            }

            // If we get here then:
            // a) we found our exception type
            // b) we fell all the way through and found Exception as the type
            return s_ExceptionTypeToEventIdMap[exceptionType];
        }
    }
}
