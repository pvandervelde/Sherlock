-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateTestStepReportDirectory]
    @id INT,
    @testStepId INT,
    @path NVARCHAR(MAX)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[TestStepReportDirectory]
        WHERE [pk_TestStepReportDirectoryId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test step report directory with ID %d does not exist.', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[TestStepReportDirectory]
        SET
            [fk_TestStepId] = @testStepId,
            [Path] = @path
        WHERE [pk_TestStepReportDirectoryId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test step report directory with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
