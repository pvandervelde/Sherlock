-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestEnvironmentsById]
    @id INT = NULL
AS
    SELECT
        [pk_TestEnvironmentId],
        [fk_TestId],
        [fk_DesiredOperatingSystemId],
        [EnvironmentName],
        [fk_SelectedMachineId]
    FROM [Tests].[TestEnvironment]
    WHERE [pk_TestEnvironmentId] = @id OR @id IS NULL
