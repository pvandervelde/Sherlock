-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[UpdateHypervMachine]
    @machineId NVARCHAR(50),
    @name NVARCHAR(50),
    @description NVARCHAR(MAX),
    @networkName NVARCHAR(50),
    @macAddress NVARCHAR(50),
    @isAvailableForTesting BIT,
    @isActive BIT,
    @operatingSystemId INT,
    @hostId NVARCHAR(50),
    @image NVARCHAR(50),
    @snapshotToReturnTo NVARCHAR(MAX)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[HypervMachine]
        WHERE [pk_HypervMachineId] = @machineId)
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

    DECLARE @isPhysicalMachine INT
    EXEC [Environments].[IsPhysicalMachine] @hostId, @isPhysicalMachine OUTPUT
    IF @isPhysicalMachine = 0
    BEGIN
        RAISERROR 
            (
                N'The machine with ID %s is not a physical machine and thus cannot be a host machine.', 
                11, 
                1,
                @hostId
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

        UPDATE [Environments].[HypervMachine]
        SET
            [Image] = @image,
            [SnapshotToReturnTo] = @snapshotToReturnTo,
            [fk_HostId] = @hostId
        WHERE [pk_HypervMachineId] = @machineId

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
