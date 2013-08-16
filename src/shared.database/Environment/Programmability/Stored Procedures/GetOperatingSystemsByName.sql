-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetOperatingSystemsByName]
    @name NVARCHAR(50) = NULL,
    @servicePack NVARCHAR(50) = NULL,
    @architecturePointerSize INT = NULL,
    @culture NVARCHAR(50) = NULL
AS
    SELECT
        [pk_OperatingSystemId],
        [Name],
        [ServicePack],
        [ArchitecturePointerSize],
        [Culture]
    FROM [Environments].[OperatingSystem]
    WHERE ([Name] = @name OR @name IS NULL)
        AND ([ServicePack] = @servicePack OR @servicePack IS NULL)
        AND ([ArchitecturePointerSize] = @architecturePointerSize OR @architecturePointerSize IS NULL)
        AND ([Culture] = @culture OR @culture IS NULL)