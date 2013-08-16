-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE TABLE [Tests].[TestEnvironment]
(
    [pk_TestEnvironmentId] INT NOT NULL PRIMARY KEY IDENTITY,
    [fk_TestId] INT NOT NULL,
    [fk_DesiredOperatingSystemId] INT NOT NULL,
    [EnvironmentName] NVARCHAR(50) NOT NULL,
    [fk_SelectedMachineId] NVARCHAR(50),
    CONSTRAINT [FK_TestEnvironment_ToTest]
        FOREIGN KEY ([fk_TestId])
        REFERENCES [Tests].[Test]([pk_TestId])
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    CONSTRAINT [FK_TestEnvironment_ToOperatingSystem]
        FOREIGN KEY ([fk_DesiredOperatingSystemId])
        REFERENCES [Environments].[OperatingSystem]([pk_OperatingSystemId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT [FK_TestEnvironment_ToMachine]
        FOREIGN KEY ([fk_SelectedMachineId])
        REFERENCES [Environments].[Machine]([pk_MachineId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
)
