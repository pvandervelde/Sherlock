//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Sherlock.Shared.DataAccess")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("Sherlock.Shared.DataAccess")]
[assembly: AssemblyCulture("")]

// Indicate that the assembly is CLS compliant.
[assembly: CLSCompliant(true)]

// Indicate that the neutral language resources are in the assembly.
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1716:IdentifiersShouldNotMatchKeywords", 
    Scope = "namespace", 
    Target = "Sherlock.Shared.DataAccess", 
    MessageId = "Shared",
    Justification = "It's a shared library. Keeping the Shared bit.")]
