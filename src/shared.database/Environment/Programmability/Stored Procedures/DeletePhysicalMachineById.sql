-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[DeletePhysicalMachineById]
    @machineId NVARCHAR(50)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[Machine] 
        WHERE [pk_MachineId] = @machineId)
    BEGIN
        RAISERROR 
            (
                N'No entry for a physical machine with ID %s has been found', 
                11, 
                1,
                @machineId
            )
        RETURN
    END

    DECLARE @isPhysicalMachine INT
    EXEC [Environments].[IsPhysicalMachine] @machineId, @isPhysicalMachine OUTPUT
    IF @isPhysicalMachine = 0
    BEGIN
        RAISERROR 
            (
                N'The machine with ID %s is not a physical machine.', 
                11, 
                1,
                @machineId
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Environments].[MachineApplication]
        WHERE [fk_MachineId] = @machineId

        DELETE FROM [Environments].[PhysicalMachine]
        WHERE [pk_PhysicalMachineId] = @machineId

        DELETE FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machineId

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete physical machine with ID %s.', 
                    11, 
                    1,
                    @machineId
                )
            RETURN
        END

    COMMIT
RETURN 0
