-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetOperatingSystemsById]
    @id int = NULL
AS
    SELECT
        [pk_OperatingSystemId],
        [Name],
        [ServicePack],
        [ArchitecturePointerSize],
        [Culture]
    FROM [Environments].[OperatingSystem]
    WHERE [pk_OperatingSystemId] = @id OR @id IS NULL

