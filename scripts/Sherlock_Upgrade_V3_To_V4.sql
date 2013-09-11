﻿/*
Deployment script for Sherlock

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
USE [Sherlock];


GO
PRINT N'Altering [Environments].[IsHypervMachine]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

ALTER PROCEDURE [Environments].[IsHypervMachine]
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
        FROM [Environments].[HypervMachine]
        WHERE [pk_HypervMachineId] = @machineId)
    BEGIN
        SET @result = CAST(0 AS BIT)
        RETURN 0
    END

    SET @result = CAST(1 AS BIT)
    RETURN 0
GO
PRINT N'Altering [Environments].[IsPhysicalMachine]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

ALTER PROCEDURE [Environments].[IsPhysicalMachine]
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
GO
PRINT N'Altering [Environments].[UpdateHypervMachine]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

ALTER PROCEDURE [Environments].[UpdateHypervMachine]
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
GO
PRINT N'Altering [Versioning].[GetSchemaVersion]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

-- Reads current database schema version.
ALTER PROCEDURE [Versioning].[GetSchemaVersion]
AS
BEGIN
    SELECT 4
END
GO
PRINT N'Altering [Environments].[AddHypervMachine]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

ALTER PROCEDURE [Environments].[AddHypervMachine]
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
GO
PRINT N'Altering [Environments].[DeleteHypervMachineById]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

ALTER PROCEDURE [Environments].[DeleteHypervMachineById]
    @machineId NVARCHAR(50)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[Machine] 
        WHERE [pk_MachineId] = @machineId)
    BEGIN
        RAISERROR 
            (
                N'No entry for a Hyper-V machine with ID %s has been found', 
                11, 
                1,
                @machineId
            )
        RETURN
    END

    DECLARE @isHypervMachine INT
    EXEC [Environments].[IsHypervMachine] @machineId, @isHypervMachine OUTPUT
    IF @isHypervMachine = 0
    BEGIN
        RAISERROR 
            (
                N'The machine with ID %s is not a Hyper-V machine.', 
                11, 
                1,
                @machineId
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Environments].[MachineApplication]
        WHERE [fk_MachineId] = @machineId

        DELETE FROM [Environments].[HypervMachine]
        WHERE [pk_HypervMachineId] = @machineId

        DELETE FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machineId

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete Hyper-V machine with ID %s.', 
                    11, 
                    1,
                    @machineId
                )
            RETURN
        END

    COMMIT
RETURN 0
GO
PRINT N'Altering [Environments].[DeletePhysicalMachineById]...';


GO
-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

ALTER PROCEDURE [Environments].[DeletePhysicalMachineById]
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
GO

GO
PRINT N'Update complete.';


GO
