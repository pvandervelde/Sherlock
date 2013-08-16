-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[DeleteTestById]
    @id int
AS
    IF NOT EXISTS (
        SELECT *
        FROM [Tests].[Test]
        WHERE [pk_TestId] = @id)
    BEGIN
        RAISERROR
            (
                N'No entry for a test with ID %d has been found', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION
        
        DELETE FROM [Tests].[Test]
        WHERE [pk_TestId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete test with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
