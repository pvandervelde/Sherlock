-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepsByTestEnvironmentId]
    @id INT
AS
    SELECT
        [pk_TestStepId]
    FROM [Tests].[TestStep]
    WHERE [fk_TestEnvironmentId] = @id
