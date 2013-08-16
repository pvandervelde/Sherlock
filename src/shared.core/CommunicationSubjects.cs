//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the communication subjects for the Sherlock application.
    /// </summary>
    public static class CommunicationSubjects
    {
        /// <summary>
        /// The communication subject for Sherlock applications that deal with test scheduling.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "CommunicationSubject is a readonly construct.")]
        public static readonly CommunicationSubject TestScheduling 
            = new CommunicationSubject("Sherlock.Subject: TestScheduling");

        /// <summary>
        /// The communication subject for Sherlock applications that deal with test data transfer.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "CommunicationSubject is a readonly construct.")]
        public static readonly CommunicationSubject TestTransfer
            = new CommunicationSubject("Sherlock.Subject: TestTransfer");

        /// <summary>
        /// The communication subject for Sherlock applications that deal with test execution.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "CommunicationSubject is a readonly construct.")]
        public static readonly CommunicationSubject TestExecution
            = new CommunicationSubject("Sherlock.Subject: TestExecution");
    }
}
