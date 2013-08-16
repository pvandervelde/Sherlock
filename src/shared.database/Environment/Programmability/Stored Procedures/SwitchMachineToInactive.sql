-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Environments].[SwitchMachineToInactive]
    @machineId NVARCHAR(50)
AS
    IF NOT EXISTS (
        SELECT * 
        FROM [Environments].[Machine]
        WHERE [pk_MachineId] = @machineId)
    BEGIN
        RAISERROR 
            (
                N'An entry for a machine with ID %s does not exist.', 
                11, 
                1,
                @machineId
            )
        RETURN
    END

    BEGIN TRANSACTION

        UPDATE [Environments].[Machine]
        SET
            [IsActive] = 0
        WHERE [pk_MachineId] = @machineId

        IF @@ERROR <> 0
        BEGIN
            ROLLBACK

            RAISERROR 
                (
                    'Failed to update machine with ID %s.', 
                    11, 
                    1,
                    @machineId
                )
            RETURN
        END

    COMMIT
RETURN 0
