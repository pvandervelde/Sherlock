﻿-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AddMsiInstallTestStep]
    @testEnvironmentId INT,
    @order INT,
    @onFailure NVARCHAR(50),
    @reportIncludesSystemLog BIT
AS
    BEGIN TRANSACTION

        INSERT INTO [Tests].[TestStep]
            (
                [fk_TestEnvironmentId],
                [Order],
                [OnFailure],
                [ReportIncludesSystemLog]
            )
        VALUES
            (
                @testEnvironmentId,
                @order,
                @onFailure,
                @reportIncludesSystemLog
            )

        DECLARE @id INT
        SELECT @id = [pk_TestStepId]
        FROM [Tests].[TestStep]
        WHERE [pk_TestStepId] = SCOPE_IDENTITY()

        INSERT INTO [Tests].[MsiInstallTestStep]
            (
                [pk_MsiInstallTestStepId]
            )
        VALUES
            (
                @id
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