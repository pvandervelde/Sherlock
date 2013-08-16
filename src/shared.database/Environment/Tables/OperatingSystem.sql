-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Environments].[OperatingSystem]
(
    [pk_OperatingSystemId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ServicePack] NVARCHAR(50) NULL, 
    [ArchitecturePointerSize] INT NOT NULL, 
    [Culture] NVARCHAR(50) NOT NULL,
    CONSTRAINT [CK_OperatingSystem_Unique] UNIQUE (Name, ServicePack, ArchitecturePointerSize, Culture)
)
