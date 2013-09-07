-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[ConsoleExecuteTestStep]
(
    [pk_ConsoleExecuteTestStepId] INT NOT NULL PRIMARY KEY,
    [ExecutableFilePath] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [FK_ConsoleExecuteTestStep_ToTestStep]
        FOREIGN KEY ([pk_ConsoleExecuteTestStepId])
        REFERENCES [Tests].[TestStep]([pk_TestStepId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
