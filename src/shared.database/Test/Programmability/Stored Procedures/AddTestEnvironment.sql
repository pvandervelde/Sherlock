-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AddTestEnvironment]
    @testId INT,
    @desiredOperatingSystemId INT,
    @name NVARCHAR(50)
AS
    IF EXISTS (
        SELECT *
        FROM [Tests].[TestEnvironment]
        WHERE 
            [fk_TestId] = @testId AND
            [EnvironmentName] = @name)
    BEGIN
        RAISERROR 
            (
                N'An entry for an environment for test %d with name %s already exists', 
                11, 
                1,
                @testId,
                @name
            )
        RETURN
    END

    BEGIN TRANSACTION

        INSERT INTO [Tests].[TestEnvironment]
            (
                [fk_TestId],
                [fk_DesiredOperatingSystemId],
                [EnvironmentName]
            )
        VALUES
            (
                @testId,
                @desiredOperatingSystemId,
                @name
            )

        DECLARE @id INT

        SELECT @id = [pk_TestEnvironmentId]
        FROM [Tests].[TestEnvironment]
        WHERE [pk_TestEnvironmentId] = SCOPE_IDENTITY()

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    N'Failed to insert an environment for test %d with name %s', 
                    11, 
                    1,
                    @testId,
                    @name
                )
            RETURN
        END

    COMMIT
    SELECT @id as Id