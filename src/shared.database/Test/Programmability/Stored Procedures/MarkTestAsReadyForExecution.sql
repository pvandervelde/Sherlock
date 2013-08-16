-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[MarkTestAsReadyForExecution]
    @id INT
AS
    IF NOT EXISTS(
        SELECT *
        FROM [Tests].[Test]
        WHERE [pk_TestId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test with id %d does not exist', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[Test]
        SET
            [IsReadyForExecution] = 1
        WHERE [pk_TestId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
