-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[DeleteConsoleExecuteTestStepById]
    @id INT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[ConsoleExecuteTestStep] 
        WHERE [pk_ConsoleExecuteTestStepId] = @id)
    BEGIN
        RAISERROR 
            (
                N'No entry for a console execute test step with ID %d has been found', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Tests].[ConsoleExecuteTestStep]
        WHERE [pk_ConsoleExecuteTestStepId] = @id

        DELETE FROM [Tests].[TestStep]
        WHERE [pk_TestStepId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete console execute test step with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0