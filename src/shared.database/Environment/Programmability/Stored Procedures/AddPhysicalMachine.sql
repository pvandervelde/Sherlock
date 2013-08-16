-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[AddPhysicalMachine]
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

        INSERT INTO [Environments].[PhysicalMachine]
            (
                [CanStartRemotely],
                [pk_PhysicalMachineId]
            )
        VALUES
            (
                @canStartRemotely,
                @machineId
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