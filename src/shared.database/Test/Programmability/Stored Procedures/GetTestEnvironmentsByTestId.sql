-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestEnvironmentsByTestId]
    @id INT
AS
    SELECT
        [pk_TestEnvironmentId]
    FROM [Tests].[TestEnvironment]
    WHERE [fk_TestId] = @id
