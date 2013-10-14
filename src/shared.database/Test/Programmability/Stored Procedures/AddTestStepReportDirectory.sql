-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AddTestStepReportDirectory]
    @testStepId INT,
    @path NVARCHAR(MAX)
AS
    BEGIN TRANSACTION
        
        INSERT INTO [Tests].[TestStepReportDirectory]
            (
                [fk_TestStepId],
                [Path]
            )
        VALUES
            (
                @testStepId,
                @path
            )

        DECLARE @id INT
        SELECT @id = [pk_TestStepReportDirectoryId]
        FROM [Tests].[TestStepReportDirectory]
        WHERE [pk_TestStepReportDirectoryId] = SCOPE_IDENTITY()

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to insert test step report directory with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
    SELECT @id as Id