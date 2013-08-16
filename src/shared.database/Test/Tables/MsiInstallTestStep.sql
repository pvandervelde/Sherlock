-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[MsiInstallTestStep]
(
    [pk_MsiInstallTestStepId] INT NOT NULL PRIMARY KEY,
    CONSTRAINT [FK_MsiInstallTestStep_ToTestStep]
        FOREIGN KEY ([pk_MsiInstallTestStepId])
        REFERENCES [Tests].[TestStep]([pk_TestStepId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
