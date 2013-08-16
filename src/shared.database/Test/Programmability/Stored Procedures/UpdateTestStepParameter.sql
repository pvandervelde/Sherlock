-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateTestStepParameter]
    @id INT,
    @testStepId INT,
    @key NVARCHAR(50),
    @value NVARCHAR(MAX)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[TestStepParameter]
        WHERE [pk_TestStepParameterId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test step parameter with ID %d does not exist.', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[TestStepParameter]
        SET
            [fk_TestStepId] = @testStepId,
            [Key] = @key,
            [Value] = @value
        WHERE [pk_TestStepParameterId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test step parameter with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
