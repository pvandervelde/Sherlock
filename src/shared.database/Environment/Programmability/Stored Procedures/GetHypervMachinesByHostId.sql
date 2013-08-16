-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetHypervMachinesByHostId]
    @id NVARCHAR(50)
AS
    SELECT
        [Environments].[Machine].[pk_MachineId],
        [Environments].[Machine].[Name],
        [Environments].[Machine].[Description],
        [Environments].[Machine].[NetworkName],
        [Environments].[Machine].[MacAddress],
        [Environments].[Machine].[IsAvailableForTesting],
        [Environments].[Machine].[IsActive],
        [Environments].[Machine].[fk_OperatingSystem],
        [Environments].[HypervMachine].[pk_HypervMachineId],
        [Environments].[HypervMachine].[Image],
        [Environments].[HypervMachine].[SnapshotToReturnTo],
        [Environments].[HypervMachine].[fk_HostId]
    FROM [Environments].[HypervMachine]
    JOIN [Environments].[Machine]
    ON [Environments].[HypervMachine].[pk_HypervMachineId] = [Environments].[Machine].[pk_MachineId]
    WHERE [Environments].[HypervMachine].[fk_HostId] = @id OR @id IS NULL
