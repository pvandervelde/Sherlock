-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetMachineApplications]
    @id int = NULL
AS
    SELECT
        [pk_MachineApplicationId],
        [fk_MachineId],
        [fk_ApplicationId]
    FROM [Environments].[MachineApplication]
    WHERE [pk_MachineApplicationId] = @id OR @id IS NULL
