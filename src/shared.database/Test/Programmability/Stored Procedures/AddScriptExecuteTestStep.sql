-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AddScriptExecuteTestStep]
    @testEnvironmentId INT,
    @order INT,
    @onFailure NVARCHAR(50),
    @language NVARCHAR(50)
AS
    BEGIN TRANSACTION

        INSERT INTO [Tests].[TestStep]
            (
                [fk_TestEnvironmentId],
                [Order],
                [OnFailure]
            )
        VALUES
            (
                @testEnvironmentId,
                @order,
                @onFailure
            )

        DECLARE @id INT
        SELECT @id = [pk_TestStepId]
        FROM [Tests].[TestStep]
        WHERE [pk_TestStepId] = SCOPE_IDENTITY()

        INSERT INTO [Tests].[ScriptExecuteTestStep]
            (
                [pk_ScriptExecuteTestStepId],
                [Language]
            )
        VALUES
            (
                @id,
                @language
            )

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to insert test step with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
    SELECT @id as Id