﻿-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[TestStepReportFile]
(
    [pk_TestStepReportFileId] INT NOT NULL PRIMARY KEY IDENTITY,
    [fk_TestStepId] INT NOT NULL,
    [Path] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [FK_TestStepReportFile_ToTestStep]
        FOREIGN KEY ([fk_TestStepId])
        REFERENCES [Tests].[TestStep]([pk_TestStepId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
)
