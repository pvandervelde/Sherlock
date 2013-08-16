-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Environments].[HypervMachine]
(
    [pk_HypervMachineId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [Image] NVARCHAR(50) NOT NULL, 
    [SnapshotToReturnTo] NVARCHAR(MAX) NOT NULL,
    [fk_HostId] NVARCHAR(50) NOT NULL,
    CONSTRAINT [FK_HypervMachine_ToMachine] 
        FOREIGN KEY ([pk_HypervMachineId]) 
        REFERENCES [Environments].[Machine]([pk_MachineId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    CONSTRAINT [FK_HypervMachine_ToHostMachine] 
        FOREIGN KEY ([fk_HostId]) 
        REFERENCES [Environments].[Machine]([pk_MachineId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
)
