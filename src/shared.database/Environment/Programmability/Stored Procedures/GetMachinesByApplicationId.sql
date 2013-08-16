-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetMachinesByApplicationId]
    @applicationId int
AS
    SELECT [pk_MachineId]
    FROM [Environments].[Machine]
    JOIN [Environments].[MachineApplication] ON [Environments].[Machine].[pk_MachineId] = [Environments].[MachineApplication].[fk_MachineId]
    WHERE [Environments].[MachineApplication].[fk_ApplicationId] = @applicationId
