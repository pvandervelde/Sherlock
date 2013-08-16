-------------------------------------------------------------------------------
-- <copyright company="Sherlock">
--     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
-- </copyright>
-------------------------------------------------------------------------------

CREATE PROCEDURE [Tests].[GetTestStepParametersByTestStepId]
    @id INT
AS
    SELECT
        [pk_TestStepParameterId]
    FROM [Tests].[TestStepParameter]
    WHERE [fk_TestStepId] = @id
