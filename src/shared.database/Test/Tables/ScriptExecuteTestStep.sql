-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[ScriptExecuteTestStep]
(
    [pk_ScriptExecuteTestStepId] INT NOT NULL PRIMARY KEY,
    [Language] NVARCHAR(50) NOT NULL,
    CONSTRAINT [FK_ScriptExecuteTestStep_ToTestStep]
        FOREIGN KEY ([pk_ScriptExecuteTestStepId])
        REFERENCES [Tests].[TestStep]([pk_TestStepId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
