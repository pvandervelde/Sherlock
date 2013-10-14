-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepReportFilesByTestStepId]
    @id INT
AS
    SELECT
        [pk_TestStepReportFileId]
    FROM [Tests].[TestStepReportFile]
    WHERE [fk_TestStepId] = @id
