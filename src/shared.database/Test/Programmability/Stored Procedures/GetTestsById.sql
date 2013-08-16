-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestsById]
    @id INT = NULL
AS
    SELECT
        [pk_TestId],
        [ProductName],
        [ProductVersion],
        [Owner],
        [TestDescription],
        [ReportPath],
        [IsReadyForExecution],
        [RequestTime],
        [StartTime],
        [EndTime]
    FROM [Tests].[Test]
    WHERE [pk_TestId] = @id OR @id IS NULL