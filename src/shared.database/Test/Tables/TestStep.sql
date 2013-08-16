-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[TestStep]
(
    [pk_TestStepId] INT NOT NULL PRIMARY KEY IDENTITY,
    [fk_TestEnvironmentId] INT NOT NULL,
    [Order] INT NOT NULL,
    CONSTRAINT [FK_TestStep_ToTestEnvironment]
        FOREIGN KEY ([fk_TestEnvironmentId])
        REFERENCES [Tests].[TestEnvironment]([pk_TestEnvironmentId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
