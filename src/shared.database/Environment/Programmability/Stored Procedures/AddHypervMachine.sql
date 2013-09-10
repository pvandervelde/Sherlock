-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[AddHypervMachine]
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
    IF EXISTS (
        SELECT * 
        FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machineId)
    BEGIN
        RAISERROR 
            (
                N'An entry for a machine with ID %s already exists.', 
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

        INSERT INTO [Environments].[Machine]
            (
                [pk_MachineId],
                [Name],
                [Description],
                [NetworkName],
                [MacAddress],
                [IsAvailableForTesting],
                [IsActive],
                [fk_OperatingSystem]
            )
        VALUES
            (
                @machineId,
                @name,
                @description,
                @networkName,
                @macAddress,
                @isAvailableForTesting,
                @isActive,
                @operatingSystemId
            )

        INSERT INTO [Environments].[HypervMachine]
            (
                [pk_HypervMachineId],
                [Image],
                [SnapshotToReturnTo],
                [fk_HostId]
            )
        VALUES
            (
                @machineId,
                @image,
                @snapshotToReturnTo,
                @hostId
            )

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to insert machine with ID %s.', 
                    11, 
                    1,
                    @machineId
                )
            RETURN
        END

    COMMIT
