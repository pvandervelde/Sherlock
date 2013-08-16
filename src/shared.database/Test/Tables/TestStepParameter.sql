-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[TestStepParameter]
(
    [pk_TestStepParameterId] INT NOT NULL PRIMARY KEY IDENTITY,
    [fk_TestStepId] INT NOT NULL,
    [Key] NVARCHAR(50) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [FK_TestStepParameter_ToTestStep]
        FOREIGN KEY ([fk_TestStepId])
        REFERENCES [Tests].[TestStep]([pk_TestStepId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
