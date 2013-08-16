-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetMachineApplicationByMachineId]
    @id NVARCHAR(50)
AS
    SELECT
        [pk_MachineApplicationId],
        [fk_MachineId],
        [fk_ApplicationId]
    FROM [Environments].[MachineApplication]
    WHERE [fk_MachineId] = @id
