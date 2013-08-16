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
[assembly: AssemblyTitle("Sherlock.Executor")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("Sherlock.Executor")]
[assembly: AssemblyCulture("")]

// Indicate that the assembly is CLS compliant.
[assembly: CLSCompliant(true)]

// Indicate that the neutral language resources are in the assembly.
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1703:ResourceStringsShouldBeSpelledCorrectly", 
    Scope = "resource", 
    Target = "Sherlock.Executor.Properties.Resources.resources", 
    MessageId = "channeltype",
    Justification = "It's a command line parameter, it's not possible to put spaces in the name.")]

[module: SuppressMessage(
    "Microsoft.Naming", 
    "CA1703:ResourceStringsShouldBeSpelledCorrectly", 
    Scope = "resource", 
    Target = "Sherlock.Executor.Properties.Resources.resources", 
    MessageId = "channeluri",
    Justification = "It's a command line parameter, it's not possible to put space in the name.")]
