-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetMsiInstallTestStepsById]
    @id INT = NULL
AS
    SELECT
        [Tests].[TestStep].[pk_TestStepId],
        [Tests].[TestStep].[fk_TestEnvironmentId],
        [Tests].[TestStep].[Order],
        [Tests].[TestStep].[OnFailure],
        [Tests].[TestStep].[ReportIncludesSystemLog]
    FROM [Tests].[MsiInstallTestStep]
    JOIN [Tests].[TestStep]
    ON [Tests].[MsiInstallTestStep].[pk_MsiInstallTestStepId] = [Tests].[TestStep].[pk_TestStepId]
    WHERE [Tests].[MsiInstallTestStep].[pk_MsiInstallTestStepId] = @id OR @id IS NULL
