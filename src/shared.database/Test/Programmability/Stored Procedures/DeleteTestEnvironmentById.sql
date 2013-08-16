-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[DeleteTestEnvironmentById]
    @id INT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[TestEnvironment] 
        WHERE [pk_TestEnvironmentId] = @id)
    BEGIN
        RAISERROR 
            (
                N'No entry for a test environment with ID %d has been found', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Tests].[TestEnvironment]
        WHERE [pk_TestEnvironmentId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete test environment with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
