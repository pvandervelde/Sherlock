-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[IsPhysicalMachine]
    @machineId NVARCHAR(50)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machineId)
    BEGIN
        SELECT CAST(0 AS BIT)
        RETURN
    END
    
    IF NOT EXISTS (
        SELECT *
        FROM [Environments].[PhysicalMachine]
        WHERE [pk_PhysicalMachineId] = @machineId)
    BEGIN
        SELECT CAST(0 AS BIT)
        RETURN
    END

    SELECT CAST(1 AS BIT)
    RETURN