-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[DeleteApplicationFromTestEnvironment]
    @testEnvironmentId INT,
    @applicationId INT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[TestApplication]
        WHERE 
            [fk_TestEnvironmentId] = @testEnvironmentId AND 
            [fk_ApplicationId] = @applicationId)
    BEGIN
        RAISERROR 
            (
                N'An entry for an environment with ID %s and an application with ID %d does not exist.', 
                11, 
                1,
                @testEnvironmentId,
                @applicationId
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Tests].[TestApplication]
        WHERE 
            [fk_TestEnvironmentId] = @testEnvironmentId AND 
            [fk_ApplicationId] = @applicationId

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update application with ID %s.', 
                    11, 
                    1,
                    @applicationId
                )
            RETURN
        END

    COMMIT
RETURN 0
