-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[UpdateTest]
    @id INT,
    @productName NVARCHAR(50),
    @productVersion NVARCHAR(50),
    @owner NVARCHAR(50),
    @description NVARCHAR(MAX),
    @reportPath NVARCHAR(MAX)
AS
    IF NOT EXISTS(
        SELECT *
        FROM [Tests].[Test]
        WHERE [pk_TestId] = @id)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test with id %d does not exist', 
                11, 
                1,
                @id
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[Test]
        SET
            [ProductName]  = @productName,
            [ProductVersion] = @productVersion,
            [Owner] = @owner,
            [TestDescription] = @description,
            [ReportPath] = @reportPath
        WHERE [pk_TestId] = @id

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test with ID %s.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
RETURN 0
