-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetMachineApplicationByApplicationId]
    @id int
AS
    SELECT
        [pk_MachineApplicationId],
        [fk_MachineId],
        [fk_ApplicationId]
    FROM [Environments].[MachineApplication]
    WHERE [fk_ApplicationId] = @id
