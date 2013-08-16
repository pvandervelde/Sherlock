-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[MarkTestAsStarted]
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

    DECLARE @isReadyForExecution BIT
    SELECT @isReadyForExecution = [IsReadyForExecution]
    FROM [Tests].[Test]
    WHERE [pk_TestId] = @id

    IF @isReadyForExecution = 0
    BEGIN
        RAISERROR 
            (
                N'Cannot start a test (%d) that is not ready for execution.', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[Test]
        SET
            [StartTime] = SYSDATETIMEOFFSET()
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
