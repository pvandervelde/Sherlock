-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[GetApplicationsById]
    @id int = NULL
AS
    SELECT 
        [pk_ApplicationId],
        [Name],
        [VersionMajor],
        [VersionMinor],
        [VersionPatch],
        [VersionBuild]
    FROM [Environments].[Application]
    WHERE [pk_ApplicationId] = @id OR @id IS NULL
