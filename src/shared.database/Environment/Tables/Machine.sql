-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Environments].[Machine]
(
    [pk_MachineId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [NetworkName] NVARCHAR(50) NOT NULL,
    [MacAddress] NVARCHAR(50) NOT NULL,
    [IsAvailableForTesting] BIT NOT NULL,
    [IsActive] BIT NOT NULL, 
    [fk_OperatingSystem] INT NOT NULL, 
    CONSTRAINT [FK_Machine_ToOperatingSystem] 
        FOREIGN KEY ([fk_OperatingSystem]) 
        REFERENCES [Environments].[OperatingSystem]([pk_OperatingSystemId]) 
        ON DELETE NO ACTION 
        ON UPDATE NO ACTION,
)
