-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateMsiInstallTestStep]
    @id INT,
    @testEnvironmentId INT,
    @order INT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[MsiInstallTestStep]
        WHERE [pk_MsiInstallTestStepId] = @id)
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
            [Order] = @order
        WHERE [pk_TestStepId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update msi install test step with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
