-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Environments].[Application]
(
    [pk_ApplicationId] INT NOT NULL PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(50) NOT NULL, 
    [VersionMajor] INT NOT NULL,
    [VersionMinor] INT NOT NULL,
    [VersionPatch] INT NOT NULL,
    [VersionBuild] INT NOT NULL, 
    CONSTRAINT [CK_Application_Unique] UNIQUE (Name, VersionMajor, VersionMinor, VersionPatch, VersionBuild),

)
