-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestApplicationsByTestEnvironmentId]
    @id INT
AS
    SELECT
        [pk_TestApplicationId]
    FROM [Tests].[TestApplication]
    WHERE [fk_TestEnvironmentId] = @id
