-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[IsPhysicalMachine]
    @machineId NVARCHAR(50),
    @result BIT OUTPUT
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
        FROM [Environments].[PhysicalMachine]
        WHERE [pk_PhysicalMachineId] = @machineId)
    BEGIN
        SET @result =  CAST(0 AS BIT)
        RETURN 0
    END

    SET @result = CAST(1 AS BIT)
    RETURN 0