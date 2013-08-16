-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetApplicationsByName]
    @name NVARCHAR(50) = NULL,
    @versionMajor INT = NULL,
    @versionMinor INT = NULL,
    @versionPatch INT = NULL,
    @versionBuild INT = NULL
AS
    SELECT 
        [pk_ApplicationId],
        [Name],
        [VersionMajor],
        [VersionMinor],
        [VersionPatch],
        [VersionBuild]
    FROM [Environments].[Application]
    WHERE ([Name] = @name OR @name IS NULL)
        AND ([VersionMajor] = @versionMajor OR @versionMajor IS NULL)
        AND ([VersionMinor] = @versionMinor OR @versionMinor IS NULL)
        AND ([VersionPatch] = @versionPatch OR @versionPatch IS NULL)
        AND ([VersionBuild] = @versionBuild OR @versionBuild IS NULL)
