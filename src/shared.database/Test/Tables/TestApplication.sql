-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[TestApplication]
(
    [pk_TestApplicationId] INT NOT NULL PRIMARY KEY IDENTITY,
    [fk_TestEnvironmentId] INT NOT NULL,
    [fk_ApplicationId] INT NOT NULL,
    CONSTRAINT [FK_TestApplication_ToTestEnvironment]
        FOREIGN KEY ([fk_TestEnvironmentId])
        REFERENCES [Tests].[TestEnvironment]([pk_TestEnvironmentId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    CONSTRAINT [FK_TestApplication_ToApplication]
        FOREIGN KEY ([fk_ApplicationId])
        REFERENCES [Environments].[Application]([pk_ApplicationId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
)
