-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestEnvironmentByMachineId]
    @id NVARCHAR(50)
AS
    SELECT
        [pk_TestEnvironmentId],
        [fk_TestId],
        [fk_DesiredOperatingSystemId],
        [EnvironmentName],
        [fk_SelectedMachineId]
    FROM [Tests].[TestEnvironment]
    WHERE [fk_SelectedMachineId] = @id