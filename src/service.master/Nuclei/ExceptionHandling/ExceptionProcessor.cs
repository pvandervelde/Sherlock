//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sherlock.Service.Master.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines the delegate used for actions that process unhandled exceptions.
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    public delegate void ExceptionProcessor(Exception exception);
}

