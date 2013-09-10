-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[IsHypervMachine]
    @machineId NVARCHAR(50),
    @result INT OUTPUT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machineId)
    BEGIN
        SET @result = CAST(0 AS BIT)
        RETURN 0
    END
    
    IF NOT EXISTS (
        SELECT *
        FROM [Environments].[HypervMachine]
        WHERE [pk_HypervMachineId] = @machineId)
    BEGIN
        SET @result = CAST(0 AS BIT)
        RETURN 0
    END

    SET @result = CAST(1 AS BIT)
    RETURN 0