-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetXCopyTestStepsById]
    @id INT = NULL
AS
    SELECT
        [Tests].[TestStep].[pk_TestStepId],
        [Tests].[TestStep].[fk_TestEnvironmentId],
        [Tests].[TestStep].[Order],
        [Tests].[TestStep].[OnFailure],
        [Tests].[XCopyTestStep].[Destination]
    FROM [Tests].[XCopyTestStep]
    JOIN [Tests].[TestStep]
    ON [Tests].[XCopyTestStep].[pk_XCopyTestStepId] = [Tests].[TestStep].[pk_TestStepId]
    WHERE [Tests].[XCopyTestStep].[pk_XCopyTestStepId] = @id OR @id IS NULL
