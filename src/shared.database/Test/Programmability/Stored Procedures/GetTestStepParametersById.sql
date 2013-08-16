-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepParametersById]
    @id INT
AS
    SELECT
        [pk_TestStepParameterId],
        [fk_TestStepId],
        [Key],
        [Value]
    FROM [Tests].[TestStepParameter]
    WHERE [pk_TestStepParameterId] = @id OR @id IS NULL
