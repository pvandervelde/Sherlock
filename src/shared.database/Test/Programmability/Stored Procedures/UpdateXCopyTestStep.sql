-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateXCopyTestStep]
    @id INT,
    @testEnvironmentId INT,
    @order INT,
    @onFailure NVARCHAR(50),
    @reportIncludesSystemLog BIT,
    @destination NVARCHAR(MAX)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[XCopyTestStep]
        WHERE [pk_XCopyTestStepId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test step with ID %d does not exist.', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[TestStep]
        SET
            [fk_TestEnvironmentId] = @testEnvironmentId,
            [Order] = @order,
            [OnFailure] = @onFailure,
            [ReportIncludesSystemLog] = @reportIncludesSystemLog
        WHERE [pk_TestStepId] = @id

        UPDATE [Tests].[XCopyTestStep]
        SET
            [Destination] = @destination
        WHERE [pk_XCopyTestStepId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update x-copy test step with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
