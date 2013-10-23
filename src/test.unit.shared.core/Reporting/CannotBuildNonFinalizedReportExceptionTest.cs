//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Sherlock.Shared.Core.Reporting
{
   [TestFixture]
   [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
               Justification = "Unit tests do not need documentation.")]
    public sealed class CannotBuildNonFinalizedReportExceptionTest : ExceptionContractVerifier<CannotBuildNonFinalizedReportException>
   {
   }
}
