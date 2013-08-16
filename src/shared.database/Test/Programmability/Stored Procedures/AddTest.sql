-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AddTest]
    @productName NVARCHAR(50),
    @productVersion NVARCHAR(50),
    @owner NVARCHAR(50),
    @description NVARCHAR(MAX),
    @reportPath NVARCHAR(MAX)
AS
    BEGIN TRANSACTION

        INSERT INTO [Tests].[Test]
            (
                [ProductName],
                [ProductVersion],
                [Owner],
                [TestDescription],
                [ReportPath],
                [IsReadyForExecution],
                [RequestTime]
            )
        VALUES
            (
                @productName,
                @productVersion,
                @owner,
                @description,
                @reportPath,
                0,
                SYSDATETIMEOFFSET()
            )

        DECLARE @id INT
        SELECT @id = [pk_TestId]
        FROM [Tests].[Test]
        WHERE [pk_TestId] = SCOPE_IDENTITY()

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to insert test with ID %d.', 
                    11, 
                    1,
                    @id
                )
            RETURN
        END

    COMMIT
    SELECT @id as Id
