-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AddTestStepParameter]
    @testStepId INT,
    @key NVARCHAR(50),
    @value NVARCHAR(MAX)
AS
    BEGIN TRANSACTION

        INSERT INTO [Tests].[TestStepParameter]
            (
                [fk_TestStepId],
                [Key],
                [Value]
            )
        VALUES
            (
                @testStepId,
                @key,
                @value
            )

        DECLARE @id INT
        SELECT @id = [pk_TestStepParameterId]
        FROM [Tests].[TestStepParameter]
        WHERE [pk_TestStepParameterId] = SCOPE_IDENTITY()

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to insert test step parameter with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
    SELECT @id as Id
