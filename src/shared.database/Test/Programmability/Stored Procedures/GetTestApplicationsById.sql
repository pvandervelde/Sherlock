-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestApplicationsById]
    @id INT = NULL
AS
    SELECT
        [pk_TestApplicationId],
        [fk_TestEnvironmentId],
        [fk_ApplicationId]
    FROM [Tests].[TestApplication]
    WHERE [pk_TestApplicationId] = @id OR @id IS NULL
