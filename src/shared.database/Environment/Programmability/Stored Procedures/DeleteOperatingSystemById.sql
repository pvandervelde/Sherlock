-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[DeleteOperatingSystemById]
    @id int
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[OperatingSystem] 
        WHERE [pk_OperatingSystemId] = @id)
    BEGIN
        RAISERROR 
            (
                N'No entry for an operating system with ID %d has been found', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION
        
        DELETE FROM [Environments].[OperatingSystem]
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
