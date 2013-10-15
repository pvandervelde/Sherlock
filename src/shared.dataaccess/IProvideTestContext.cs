//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sherlock.Shared.DataAccess
{
    /// <summary>
    /// Defines the interface for classes that provide access to stored test data.
    /// </summary>
    public interface IProvideTestContext : IProvideStoredContext
    {
        /// <summary>
        /// Adds a new test.
        /// </summary>
        /// <param name="test">The test.</param>
        void Add(Test test);

        /// <summary>
        /// Returns a collection containing all the known tests.
        /// </summary>
        /// <returns>A collection containing all the known tests.</returns>
        IEnumerable<Test> Tests();

        /// <summary>
        /// Returns a collection containing all the inactive tests.
        /// </summary>
        /// <returns>A collection containing all the inactive tests.</returns>
        IEnumerable<Test> InactiveTests();

        /// <summary>
        /// Returns the test with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        /// <returns>The requested test.</returns>
        Test Test(int id);

        /// <summary>
        /// Marks a test as ready for execution.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        void TestIsReadyForExecution(int id);

        /// <summary>
        /// Marks a test as started.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        void StartTest(int id);

        /// <summary>
        /// Marks a test as stopped.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        void StopTest(int id);

        /// <summary>
        /// Updates the test with the data from the given object.
        /// </summary>
        /// <param name="test">The test.</param>
        void Update(Test test);

        /// <summary>
        /// Deletes an existing test.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        void DeleteTest(int id);

        /// <summary>
        /// Adds a new test environment.
        /// </summary>
        /// <param name="environment">The test environment.</param>
        void Add(TestEnvironment environment);

        /// <summary>
        /// Returns a collection containing all the known test environments.
        /// </summary>
        /// <returns>A collection containing all the known test environments.</returns>
        IEnumerable<TestEnvironment> TestEnvironments();

        /// <summary>
        /// Returns the test environment with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the test environment.</param>
        /// <returns>The requested test environment.</returns>
        TestEnvironment TestEnvironment(int id);

        /// <summary>
        /// Marks the given environment as supported by the machine.
        /// </summary>
        /// <param name="environment">The ID of the test environment.</param>
        /// <param name="machine">The ID of the machine.</param>
        void TestEnvironmentSupportedByMachine(int environment, string machine);

        /// <summary>
        /// Updates the test environment with the data from the given object.
        /// </summary>
        /// <param name="environment">The test environment.</param>
        void Update(TestEnvironment environment);

        /// <summary>
        /// Deletes an existing test environment.
        /// </summary>
        /// <param name="id">The ID of the test environment.</param>
        void DeleteTestEnvironment(int id);

        /// <summary>
        /// Adds a new console execute test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        void Add(ConsoleExecuteTestStep testStep);

        /// <summary>
        /// Adds a new MSI install test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        void Add(MsiInstallTestStep testStep);

        /// <summary>
        /// Adds a new script executing test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        void Add(ScriptExecuteTestStep testStep);

        /// <summary>
        /// Adds a new x-copy test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        void Add(XCopyTestStep testStep);

        /// <summary>
        /// Returns a collection containing all the known test steps.
        /// </summary>
        /// <returns>A collection containing all the known test steps.</returns>
        IEnumerable<TestStep> TestSteps();

        /// <summary>
        /// Returns a non-ordered collection containing all the test steps for the given environment.
        /// </summary>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns>A non-ordered collection of test steps for the given environment.</returns>
        IEnumerable<TestStep> TestStepsForEnvironment(int environmentId);
            
        /// <summary>
        /// Returns the test step with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the test step.</param>
        /// <returns>The requested test step.</returns>
        TestStep TestStep(int id);

        /// <summary>
        /// Updates the test environment with the data from the given object.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        void Update(TestStep testStep);

        /// <summary>
        /// Deletes an existing test step.
        /// </summary>
        /// <param name="id">The ID of the test step.</param>
        void DeleteTestStep(int id);

        /// <summary>
        /// Adds a new test step parameter.
        /// </summary>
        /// <param name="parameter">The new parameter.</param>
        void Add(TestStepParameter parameter);

        /// <summary>
        /// Updates the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter that should be updated.</param>
        void Update(TestStepParameter parameter);

        /// <summary>
        /// Deletes an existing test step parameter.
        /// </summary>
        /// <param name="id">The ID of the parameter.</param>
        void DeleteTestStepParameter(int id);

        /// <summary>
        /// Adds a new test step report directory.
        /// </summary>
        /// <param name="reportDirectory">The new report directory.</param>
        void Add(TestStepReportDirectory reportDirectory);

        /// <summary>
        /// Updates the given report directory.
        /// </summary>
        /// <param name="reportDirectory">The report directory that should be updated.</param>
        void Update(TestStepReportDirectory reportDirectory);

        /// <summary>
        /// Deletes an existing test step report directory.
        /// </summary>
        /// <param name="id">The ID of the report directory.</param>
        void DeleteTestStepReportDirectory(int id);

        /// <summary>
        /// Adds a new test step report file.
        /// </summary>
        /// <param name="reportFile">The new report file.</param>
        void Add(TestStepReportFile reportFile);

        /// <summary>
        /// Updates the given report file.
        /// </summary>
        /// <param name="reportFile">The report file that should be updated.</param>
        void Update(TestStepReportFile reportFile);

        /// <summary>
        /// Deletes an existing test step report file.
        /// </summary>
        /// <param name="id">The ID of the report file.</param>
        void DeleteTestStepReportFile(int id);

        /// <summary>
        /// Adds a new test application.
        /// </summary>
        /// <param name="application">The new test application.</param>
        void Add(TestApplication application);

        /// <summary>
        /// Updates the given application.
        /// </summary>
        /// <param name="application">The test application that should be updated.</param>
        void Update(TestApplication application);

        /// <summary>
        /// Deletes an existing test application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        void DeleteTestApplication(int id);
    }
}
