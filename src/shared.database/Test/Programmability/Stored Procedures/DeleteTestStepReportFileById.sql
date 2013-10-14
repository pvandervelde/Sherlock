-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[DeleteTestStepReportFileById]
    @id INT
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Tests].[TestStepReportFile] 
        WHERE [pk_TestStepReportFileId] = @id)
    BEGIN
        RAISERROR 
            (
                N'No entry for a test step report file with ID %d has been found', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        DELETE FROM [Tests].[TestStepReportFile]
        WHERE [pk_TestStepReportFileId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to delete test step report file with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
