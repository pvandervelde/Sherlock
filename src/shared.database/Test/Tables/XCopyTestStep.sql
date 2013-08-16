-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[XCopyTestStep]
(
    [pk_XCopyTestStepId] INT NOT NULL PRIMARY KEY,
    [Destination] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [FK_XCopyTestStep_ToTestStep]
        FOREIGN KEY ([pk_XCopyTestStepId])
        REFERENCES [Tests].[TestStep]([pk_TestStepId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
