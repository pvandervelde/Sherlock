-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Environments].[PhysicalMachine]
(
    [pk_PhysicalMachineId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [CanStartRemotely] BIT NOT NULL,
    CONSTRAINT [FK_PhysicalMachine_ToMachine] 
        FOREIGN KEY ([pk_PhysicalMachineId]) 
        REFERENCES [Environments].[Machine]([pk_MachineId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
