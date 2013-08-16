-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateTestEnvironment]
    @id INT,
    @testId INT,
    @desiredOperatingSystemId INT,
    @name NVARCHAR(50)
AS
    IF NOT EXISTS(
        SELECT *
        FROM [Tests].[TestEnvironment]
        WHERE [pk_TestEnvironmentId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test environment with id %d does not exist', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[TestEnvironment]
        SET
            [fk_TestId] = @testId,
            [fk_DesiredOperatingSystemId] = @desiredOperatingSystemId,
            [EnvironmentName] = @name
        WHERE [pk_TestEnvironmentId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test environment with ID %s.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
