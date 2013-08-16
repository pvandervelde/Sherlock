-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[Test]
(
    [pk_TestId] INT NOT NULL PRIMARY KEY IDENTITY,
    [ProductName] NVARCHAR(50) NOT NULL,
    [ProductVersion] NVARCHAR(50) NOT NULL,
    [Owner] NVARCHAR(50) NOT NULL,
    [TestDescription] NVARCHAR(MAX) NOT NULL,
    [ReportPath] NVARCHAR(MAX) NOT NULL,
    [IsReadyForExecution] BIT NOT NULL,
    [RequestTime] DATETIMEOFFSET NOT NULL,
    [StartTime] DATETIMEOFFSET,
    [EndTime] DATETIMEOFFSET,
)
