-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[DeleteApplicationById]
    @id int
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[Application] 
        WHERE [pk_ApplicationId] = @id)
    BEGIN
        RAISERROR 
            (
                N'No entry for an application with ID %d has been found', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Environments].[MachineApplication]
        WHERE [fk_ApplicationId] = @id

        DELETE FROM [Environments].[Application]
        WHERE [pk_ApplicationId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete application with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
