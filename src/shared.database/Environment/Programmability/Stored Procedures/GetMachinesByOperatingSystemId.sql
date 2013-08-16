-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetMachinesByOperatingSystemId]
    @operatingSystemId int = 0
AS
    SELECT [pk_MachineId]
    FROM [Environments].[Machine]
    WHERE [fk_OperatingSystem] = @operatingSystemId
