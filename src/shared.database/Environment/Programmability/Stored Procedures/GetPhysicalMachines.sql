-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetPhysicalMachines]
    @id NVARCHAR(50) = NULL
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
        [Environments].[PhysicalMachine].[pk_PhysicalMachineId],
        [Environments].[PhysicalMachine].[CanStartRemotely]
    FROM [Environments].[PhysicalMachine] 
    JOIN [Environments].[Machine] 
    ON [Environments].[PhysicalMachine].[pk_PhysicalMachineId] = [Environments].[Machine].[pk_MachineId]
    WHERE [Environments].[Machine].[pk_MachineId] = @id OR @id IS NULL
