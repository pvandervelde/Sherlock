-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[AttachMachineToTestEnvironment]
    @environment INT,
    @machine NVARCHAR(50)
AS
    IF NOT EXISTS(
        SELECT *
        FROM [Tests].[TestEnvironment]
        WHERE [pk_TestEnvironmentId] = @environment)
    BEGIN
        RAISERROR 
            (
                N'An entry for a test environment with id %d does not exist', 
                11, 
                1,
                @environment
            )
        RETURN
    END

    IF NOT EXISTS(
        SELECT *
        FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machine)
    BEGIN
        RAISERROR 
            (
                N'An entry for a machine with id %s does not exist', 
                11, 
                1,
                @machine
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Tests].[TestEnvironment]
        SET
            [fk_SelectedMachineId] = @machine
        WHERE [pk_TestEnvironmentId] = @environment

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update test environment with ID %d.', 
                    11, 
                    1,
                    @environment
                )
            RETURN
        END

    COMMIT
RETURN 0
