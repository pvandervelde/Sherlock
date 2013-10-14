-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepReportFilesById]
    @id INT
AS
    SELECT
        [pk_TestStepReportFileId],
        [fk_TestStepId],
        [Path]
    FROM [Tests].[TestStepReportFile]
    WHERE [pk_TestStepReportFileId] = @id OR @id IS NULL
