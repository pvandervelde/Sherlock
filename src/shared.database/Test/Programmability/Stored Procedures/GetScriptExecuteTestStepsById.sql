-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetScriptExecuteTestStepsById]
    @id INT = NULL
AS
    SELECT
        [Tests].[TestStep].[pk_TestStepId],
        [Tests].[TestStep].[fk_TestEnvironmentId],
        [Tests].[TestStep].[Order],
        [Tests].[TestStep].[OnFailure],
        [Tests].[TestStep].[ReportIncludesSystemLog],
        [Tests].[ScriptExecuteTestStep].[Language]
    FROM [Tests].[ScriptExecuteTestStep]
    JOIN [Tests].[TestStep]
    ON [Tests].[ScriptExecuteTestStep].[pk_ScriptExecuteTestStepId] = [Tests].[TestStep].[pk_TestStepId]
    WHERE [Tests].[ScriptExecuteTestStep].[pk_ScriptExecuteTestStepId] = @id OR @id IS NULL
