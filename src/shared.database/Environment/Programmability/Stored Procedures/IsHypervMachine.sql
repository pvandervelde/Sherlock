-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[IsHypervMachine]
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
        FROM [Environments].[HypervMachine]
        WHERE [pk_HypervMachineId] = @machineId)
    BEGIN
        SELECT CAST(0 AS BIT)
        RETURN
    END

    SELECT CAST(1 AS BIT)
    RETURN