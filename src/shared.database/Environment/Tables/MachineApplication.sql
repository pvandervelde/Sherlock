-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Environments].[MachineApplication]
(
    [pk_MachineApplicationId] INT NOT NULL PRIMARY KEY IDENTITY,
    [fk_MachineId] NVARCHAR(50) NOT NULL,
    [fk_ApplicationId] INT NOT NULL,
    CONSTRAINT [FK_MachineApplication_ToMachine] 
        FOREIGN KEY ([fk_MachineId]) 
        REFERENCES [Environments].[Machine]([pk_MachineId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    CONSTRAINT [FK_MachineApplication_ToApplication] 
        FOREIGN KEY ([fk_ApplicationId]) 
        REFERENCES [Environments].[Application]([pk_ApplicationId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
)
