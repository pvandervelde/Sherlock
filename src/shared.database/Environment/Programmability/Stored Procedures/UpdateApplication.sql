-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[UpdateApplication]
    @id INT,
    @name NVARCHAR(50),
    @major INT,
    @minor INT,
    @patch INT,
    @build INT
AS
    IF NOT EXISTS(
        SELECT *
        FROM [Environments].[Application]
        WHERE [pk_ApplicationId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for an application %s (%d.%d.%d.%d) does not exist', 
                11, 
                1,
                @name,
                @major,
                @minor,
                @patch,
                @build
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Environments].[Application]
        SET
            [Name]  = @name,
            [VersionMajor] = @major,
            [VersionMinor] = @minor,
            [VersionPatch] = @patch,
            [VersionBuild] = @build
        WHERE [pk_ApplicationId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update application with ID %s.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
