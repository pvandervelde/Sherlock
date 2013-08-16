-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[UpdateOperatingSystem]
    @id int,
    @name NVARCHAR(50),
    @servicePack NVARCHAR(50),
    @architecturePointerSize INT,
    @culture NVARCHAR(50)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[OperatingSystem] 
        WHERE [pk_OperatingSystemId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for an operating system %s (%s) - %d - %s does not exist', 
                11, 
                1,
                @name,
                @servicePack,
                @architecturePointerSize,
                @culture
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Environments].[OperatingSystem]
        SET
            [Name] = @name,
            [ServicePack] = @servicePack,
            [ArchitecturePointerSize] = @architecturePointerSize,
            [Culture] = @culture
        WHERE [pk_OperatingSystemId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update operating system with ID %s.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
