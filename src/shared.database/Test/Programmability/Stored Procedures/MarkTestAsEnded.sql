-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[MarkTestAsEnded]
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

    DECLARE @startTime DATETIMEOFFSET
    SELECT @startTime = [StartTime]
    FROM [Tests].[Test]
    WHERE [pk_TestId] = @id

    IF @startTime IS NULL
    BEGIN
        RAISERROR 
            (
                N'Cannot stop a test (%d) that was never started.', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[Test]
        SET
            [EndTime] = SYSDATETIMEOFFSET()
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
