-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepReportDirectoriesById]
    @id INT
AS
    SELECT
        [pk_TestStepReportDirectoryId],
        [fk_TestStepId],
        [Path]
    FROM [Tests].[TestStepReportDirectory]
    WHERE [pk_TestStepReportDirectoryId] = @id OR @id IS NULL
