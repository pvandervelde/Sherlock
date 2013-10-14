-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepReportDirectoriesByTestStepId]
    @id INT
AS
    SELECT
        [pk_TestStepReportDirectoryId]
    FROM [Tests].[TestStepReportDirectory]
    WHERE [fk_TestStepId] = @id
