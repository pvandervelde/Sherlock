-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetConsoleExecuteTestStepsById]
    @id INT = NULL
AS
    SELECT
        [Tests].[TestStep].[pk_TestStepId],
        [Tests].[TestStep].[fk_TestEnvironmentId],
        [Tests].[TestStep].[Order],
        [Tests].[TestStep].[OnFailure],
        [Tests].[TestStep].[ReportIncludesSystemLog],
        [Tests].[ConsoleExecuteTestStep].[ExecutableFilePath]
    FROM [Tests].[ConsoleExecuteTestStep]
    JOIN [Tests].[TestStep]
    ON [Tests].[ConsoleExecuteTestStep].[pk_ConsoleExecuteTestStepId] = [Tests].[TestStep].[pk_TestStepId]
    WHERE [Tests].[ConsoleExecuteTestStep].[pk_ConsoleExecuteTestStepId] = @id OR @id IS NULL