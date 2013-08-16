-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[UpdatePhysicalMachine]
    @machineId NVARCHAR(50),
    @name NVARCHAR(50),
    @description NVARCHAR(MAX),
    @networkName NVARCHAR(50),
    @macAddress NVARCHAR(50),
    @isAvailableForTesting BIT,
    @isActive BIT,
    @operatingSystemId INT,
    @canStartRemotely BIT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[PhysicalMachine]
        WHERE [pk_PhysicalMachineId] = @machineId)
    BEGIN
        RAISERROR 
            (
                N'An entry for a machine with ID %s does not exist.', 
                11, 
                1,
                @machineId
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Environments].[Machine]
        SET
            [Name] = @name,
            [Description] = @description,
            [NetworkName] = @networkName,
            [MacAddress] = @macAddress,
            [IsAvailableForTesting] = @isAvailableForTesting,
            [IsActive] = @isActive,
            [fk_OperatingSystem] = @operatingSystemId
        WHERE [pk_MachineId] = @machineId

        UPDATE [Environments].[PhysicalMachine]
        SET
            [CanStartRemotely] = @canStartRemotely
        WHERE [pk_PhysicalMachineId] = @machineId

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update machine with ID %s.', 
                    11, 
                    1,
                    @machineId
                )
            RETURN
        END

    COMMIT
RETURN 0
