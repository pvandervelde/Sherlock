-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateTestStepReportFile]
    @id INT,
    @testStepId INT,
    @path NVARCHAR(MAX)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[TestStepReportFile]
        WHERE [pk_TestStepReportFileId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test step report file with ID %d does not exist.', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[TestStepReportFile]
        SET
            [fk_TestStepId] = @testStepId,
            [Path] = @path
        WHERE [pk_TestStepReportFileId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test step report file with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0