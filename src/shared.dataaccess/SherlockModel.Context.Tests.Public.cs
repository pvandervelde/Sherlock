//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Sherlock.Shared.DataAccess
{
    /// <content>
    /// Defines the public Test API for the Sherlock Context class.
    /// </content>
    internal sealed partial class SherlockContext : IProvideTestContext
    {
        private static bool Update(TestStep stored, TestStep changed)
        {
            stored.Order = changed.Order;
            stored.OnFailure = changed.OnFailure;
            if (stored.TestEnvironmentId != changed.TestEnvironmentId)
            {
                stored.TestEnvironment = null;
                stored.TestEnvironmentId = changed.TestEnvironmentId;

                return true;
            }

            return false;
        }

        private void InitializeTests()
        {
            var testApplications = GetTestApplicationsById(null).ToList();
            foreach (var testApplication in testApplications)
            {
                Patch(testApplication);
            }

            var testEnvironments = GetTestEnvironmentsById(null).ToList();
            foreach (var testEnvironment in testEnvironments)
            {
                Patch(testEnvironment);
            }

            var testStepParameters = GetTestStepParametersById(null).ToList();
            foreach (var testStepParameter in testStepParameters)
            {
                Patch(testStepParameter);
            }

            var consoleTestSteps = GetConsoleExecuteTestStepsById(null).ToList();
            foreach (var consoleTestStep in consoleTestSteps)
            {
                Patch(consoleTestStep);
            }

            var msiTestSteps = GetMsiInstallTestStepsById(null).ToList();
            foreach (var msiTestStep in msiTestSteps)
            {
                Patch(msiTestStep);
            }

            var scriptTestSteps = GetScriptExecuteTestStepsById(null).ToList();
            foreach (var scriptTestStep in scriptTestSteps)
            {
                Patch(scriptTestStep);
            }

            var xcopyTestSteps = GetXCopyTestStepsById(null).ToList();
            foreach (var xcopyTestStep in xcopyTestSteps)
            {
                Patch(xcopyTestStep);
            }

            var tests = GetTestsById(null).ToList();
            foreach (var test in tests)
            {
                Patch(test);
            }
        }

        private TestApplication Patch(TestApplication testApplication)
        {
            var result = StoredTestApplications.Find(testApplication.Id) ?? StoredTestApplications.Add(testApplication);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedApplication = GetApplicationsById(result.ApplicationId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.Application = selectedApplication;

                    var selectedTestEnvironment = GetTestEnvironmentsById(result.TestEnvironmentId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.TestEnvironment = selectedTestEnvironment;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private TestEnvironment Patch(TestEnvironment testEnvironment)
        {
            var result = StoredTestEnvironments.Find(testEnvironment.Id) ?? StoredTestEnvironments.Add(testEnvironment);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    if (result.SelectedMachineId != null)
                    {
                        var selectedMachine = Machine(result.SelectedMachineId);
                        result.Machine = selectedMachine;
                    }

                    var selectedOperatingSystem = GetOperatingSystemsById(result.DesiredOperatingSystemId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.OperatingSystem = selectedOperatingSystem;

                    var selectedTest = GetTestsById(result.TestId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.Test = selectedTest;

                    var selectedApplications = GetTestApplicationsByTestEnvironmentId(result.Id)
                        .Select(id => TestApplication(id.Value))
                        .ToList();
                    result.TestApplications = selectedApplications;

                    var selectedTestSteps = GetTestStepsByTestEnvironmentId(result.Id)
                        .Select(id => TestStep(id.Value))
                        .ToList();
                    result.TestSteps = selectedTestSteps;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private TestStepParameter Patch(TestStepParameter testStepParameter)
        {
            var result = StoredTestStepParameters.Find(testStepParameter.Id) ?? StoredTestStepParameters.Add(testStepParameter);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedTestStep = TestStep(result.fk_TestStepId);
                    result.TestStep = selectedTestStep;
                    
                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private TestStep Patch(TestStep testStep)
        {
            var result = StoredTestSteps.Find(testStep.Id) ?? StoredTestSteps.Add(testStep);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedTestEnvironment = GetTestEnvironmentsById(result.TestEnvironmentId)
                        .Select(Patch)
                        .FirstOrDefault();
                    result.TestEnvironment = selectedTestEnvironment;

                    var selectedParameters = GetTestStepParametersByTestStepId(result.Id)
                        .Select(id => TestStepParameter(id.Value))
                        .ToList();
                    result.TestStepParameters = selectedParameters;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private ConsoleExecuteTestStep Patch(ConsoleExecuteTestStep consoleTestStep)
        {
            var result = Patch((TestStep)consoleTestStep) as ConsoleExecuteTestStep;
            Debug.Assert(result != null, "The test step should be an console execute test step.");

            return result;
        }

        private MsiInstallTestStep Patch(MsiInstallTestStep msiTestStep)
        {
            var result = Patch((TestStep)msiTestStep) as MsiInstallTestStep;
            Debug.Assert(result != null, "The test step should be an MSI install test step.");

            return result;
        }

        private ScriptExecuteTestStep Patch(ScriptExecuteTestStep scriptTestStep)
        {
            var result = Patch((TestStep)scriptTestStep) as ScriptExecuteTestStep;
            Debug.Assert(result != null, "The test step should be a script execute test step.");

            return result;
        }

        private XCopyTestStep Patch(XCopyTestStep xcopyTestStep)
        {
            var result = Patch((TestStep)xcopyTestStep) as XCopyTestStep;
            Debug.Assert(result != null, "The test step should be an x-copy test step.");

            return result;
        }

        private Test Patch(Test test)
        {
            var result = StoredTests.Find(test.Id) ?? StoredTests.Add(test);
            if (!result.IsPatched && !result.IsPatching)
            {
                result.IsPatching = true;
                try
                {
                    var selectedEnvironments = GetTestEnvironmentsByTestId(result.Id)
                        .Select(id => TestEnvironment(id.Value))
                        .ToList();
                    result.TestEnvironments = selectedEnvironments;

                    result.IsPatched = true;
                }
                finally
                {
                    result.IsPatching = false;
                }
            }

            return result;
        }

        private TestApplication TestApplication(int id)
        {
            var application = StoredTestApplications.Find(id);
            if (application != null)
            {
                Patch(application);
            }

            return application;
        }

        private TestStepParameter TestStepParameter(int id)
        {
            var parameter = StoredTestStepParameters.Find(id);
            if (parameter != null)
            {
                Patch(parameter);
            }

            return parameter;
        }

        /// <summary>
        /// Adds a new test.
        /// </summary>
        /// <param name="test">The test.</param>
        public void Add(Test test)
        {
            VerifySchemaVersion();

            var result = StoredTests.Add(test);
            Patch(result);
        }

        /// <summary>
        /// Returns a collection containing all the known tests.
        /// </summary>
        /// <returns>A collection containing all the known tests.</returns>
        public IEnumerable<Test> Tests()
        {
            VerifySchemaVersion();

            return StoredTests
                .ToList();
        }

        /// <summary>
        /// Returns a collection containing all the inactive tests.
        /// </summary>
        /// <returns>A collection containing all the inactive tests.</returns>
        public IEnumerable<Test> InactiveTests()
        {
            VerifySchemaVersion();

            return GetNotExecutedTests()
                .Select(Patch)
                .ToList();
        }

        /// <summary>
        /// Returns the test with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        /// <returns>The requested test.</returns>
        public Test Test(int id)
        {
            VerifySchemaVersion();

            var test = StoredTests.Find(id) ?? GetTestsById(id).FirstOrDefault();
            return test != null ? Patch(test) : null;
        }

        /// <summary>
        /// Marks a test as ready for execution.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        public void TestIsReadyForExecution(int id)
        {
            VerifySchemaVersion();

            MarkTestAsReadyForExecution(id);
        }

        /// <summary>
        /// Marks a test as started.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        public void StartTest(int id)
        {
            VerifySchemaVersion();

            MarkTestAsStarted(id);
        }

        /// <summary>
        /// Marks a test as stopped.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        public void StopTest(int id)
        {
            VerifySchemaVersion();

            MarkTestAsEnded(id);
        }

        /// <summary>
        /// Updates the test with the data from the given object.
        /// </summary>
        /// <param name="test">The test.</param>
        public void Update(Test test)
        {
            VerifySchemaVersion();

            var storedTest = Test(test.Id);
            if (storedTest != null)
            {
                if (!ReferenceEquals(storedTest, test))
                {
                    storedTest.ProductName = test.ProductName;
                    storedTest.ProductVersion = test.ProductVersion;
                    storedTest.TestDescription = test.TestDescription;
                    storedTest.Owner = test.Owner;
                    storedTest.ReportPath = test.ReportPath;
                    storedTest.RequestTime = test.RequestTime;
                    storedTest.StartTime = test.StartTime;
                    storedTest.EndTime = test.EndTime;
                }

                var entry = Entry(storedTest);
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes an existing test.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        public void DeleteTest(int id)
        {
            VerifySchemaVersion();

            var storedTest = Test(id);
            if (storedTest != null)
            {
                StoredTests.Remove(storedTest);
            }
        }

        /// <summary>
        /// Adds a new test environment.
        /// </summary>
        /// <param name="environment">The test environment.</param>
        public void Add(TestEnvironment environment)
        {
            VerifySchemaVersion();

            var result = StoredTestEnvironments.Add(environment);
            Patch(result);
        }

        /// <summary>
        /// Returns a collection containing all the known test environments.
        /// </summary>
        /// <returns>A collection containing all the known test environments.</returns>
        public IEnumerable<TestEnvironment> TestEnvironments()
        {
            VerifySchemaVersion();

            return StoredTestEnvironments
                .ToList();
        }

        /// <summary>
        /// Returns the test environment with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the test environment.</param>
        /// <returns>The requested test environment.</returns>
        public TestEnvironment TestEnvironment(int id)
        {
            VerifySchemaVersion();

            var storedEnvironment = StoredTestEnvironments.Find(id) ?? GetTestEnvironmentsById(id).FirstOrDefault();
            return storedEnvironment != null ? Patch(storedEnvironment) : null;
        }

        /// <summary>
        /// Marks the given environment as supported by the machine.
        /// </summary>
        /// <param name="environment">The ID of the test environment.</param>
        /// <param name="machine">The ID of the machine.</param>
        public void TestEnvironmentSupportedByMachine(int environment, string machine)
        {
            VerifySchemaVersion();

            AttachMachineToTestEnvironment(environment, machine);
        }

        /// <summary>
        /// Updates the test environment with the data from the given object.
        /// </summary>
        /// <param name="environment">The test environment.</param>
        public void Update(TestEnvironment environment)
        {
            VerifySchemaVersion();

            var storedEnvironment = TestEnvironment(environment.Id);
            if (storedEnvironment != null)
            {
                bool shouldPatch = false;
                if (!ReferenceEquals(storedEnvironment, environment))
                {
                    storedEnvironment.EnvironmentName = environment.EnvironmentName;
                    if (storedEnvironment.fk_DesiredOperatingSystemId != environment.fk_DesiredOperatingSystemId)
                    {
                        storedEnvironment.fk_DesiredOperatingSystemId = environment.fk_DesiredOperatingSystemId;
                        shouldPatch = true;
                    }

                    if (storedEnvironment.fk_TestId != environment.fk_TestId)
                    {
                        storedEnvironment.fk_TestId = environment.fk_TestId;
                        shouldPatch = true;
                    }
                }

                if (shouldPatch)
                {
                    storedEnvironment.IsPatched = false;
                    Patch(storedEnvironment);
                }

                var entry = Entry(storedEnvironment);
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes an existing test environment.
        /// </summary>
        /// <param name="id">The ID of the test environment.</param>
        public void DeleteTestEnvironment(int id)
        {
            VerifySchemaVersion();

            var environment = TestEnvironment(id);
            if (environment != null)
            {
                StoredTestEnvironments.Remove(environment);
            }
        }

        /// <summary>
        /// Adds a new console execute test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        public void Add(ConsoleExecuteTestStep testStep)
        {
            VerifySchemaVersion();
            var result = StoredTestSteps.Add(testStep) as ConsoleExecuteTestStep;
            Debug.Assert(result != null, "The test step should be a console execute test step.");

            Patch(result);
        }

        /// <summary>
        /// Adds a new MSI install test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        public void Add(MsiInstallTestStep testStep)
        {
            VerifySchemaVersion();

            var result = StoredTestSteps.Add(testStep) as MsiInstallTestStep;
            Debug.Assert(result != null, "The test step should be a MSI install test step.");

            Patch(result);
        }

        /// <summary>
        /// Adds a new script executing test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        public void Add(ScriptExecuteTestStep testStep)
        {
            VerifySchemaVersion();

            var result = StoredTestSteps.Add(testStep) as ScriptExecuteTestStep;
            Debug.Assert(result != null, "The test step should be a script execute test step.");

            Patch(result);
        }

        /// <summary>
        /// Adds a new x-copy test step.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        public void Add(XCopyTestStep testStep)
        {
            VerifySchemaVersion();

            var result = StoredTestSteps.Add(testStep) as XCopyTestStep;
            Debug.Assert(result != null, "The test step should be a x-copy test step.");

            Patch(result);
        }

        /// <summary>
        /// Returns a collection containing all the known test steps.
        /// </summary>
        /// <returns>A collection containing all the known test steps.</returns>
        public IEnumerable<TestStep> TestSteps()
        {
            VerifySchemaVersion();

            return StoredTestSteps.ToList();
        }

        /// <summary>
        /// Returns a non-ordered collection containing all the test steps for the given environment.
        /// </summary>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <returns>A non-ordered collection of test steps for the given environment.</returns>
        public IEnumerable<TestStep> TestStepsForEnvironment(int environmentId)
        {
            VerifySchemaVersion();

            return GetTestStepsByTestEnvironmentId(environmentId)
                .Select(id => TestStep(id.Value))
                .ToList();
        }

        /// <summary>
        /// Returns the test step with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the test step.</param>
        /// <returns>The requested test step.</returns>
        public TestStep TestStep(int id)
        {
            VerifySchemaVersion();

            var testStep = StoredTestSteps.Find(id);
            if (testStep != null)
            {
                return testStep;
            }

            var consoleTestStep = GetConsoleExecuteTestStepsById(id).FirstOrDefault();
            if (consoleTestStep != null)
            {
                return Patch(consoleTestStep);
            }

            var msiTestStep = GetMsiInstallTestStepsById(id).FirstOrDefault();
            if (msiTestStep != null)
            {
                return Patch(msiTestStep);
            }

            var scriptTestStep = GetScriptExecuteTestStepsById(id).FirstOrDefault();
            if (scriptTestStep != null)
            {
                return Patch(scriptTestStep);
            }

            var xcopyTestStep = GetXCopyTestStepsById(id).FirstOrDefault();
            if (xcopyTestStep != null)
            {
                return Patch(xcopyTestStep);
            }

            return null;
        }

        /// <summary>
        /// Updates the test environment with the data from the given object.
        /// </summary>
        /// <param name="testStep">The test step.</param>
        public void Update(TestStep testStep)
        {
            VerifySchemaVersion();

            var storedTestStep = TestStep(testStep.Id);
            if (storedTestStep != null)
            {
                if (!ReferenceEquals(storedTestStep, testStep))
                {
                    var storedConsoleExecuteTestStep = storedTestStep as ConsoleExecuteTestStep;
                    if (storedConsoleExecuteTestStep != null)
                    {
                        Update(storedConsoleExecuteTestStep, testStep as ConsoleExecuteTestStep);
                    }

                    var storedMsiInstallTestStep = storedTestStep as MsiInstallTestStep;
                    if (storedMsiInstallTestStep != null)
                    {
                        Update(storedMsiInstallTestStep, testStep as MsiInstallTestStep);
                    }

                    var storedScriptExecuteTestStep = storedTestStep as ScriptExecuteTestStep;
                    if (storedScriptExecuteTestStep != null)
                    {
                        Update(storedScriptExecuteTestStep, testStep as ScriptExecuteTestStep);
                    }

                    var storedXCopyTestStep = storedTestStep as XCopyTestStep;
                    if (storedXCopyTestStep != null)
                    {
                        Update(storedXCopyTestStep, testStep as XCopyTestStep);
                    }
                }

                var entry = Entry(storedTestStep);
                entry.State = EntityState.Modified;
            }
        }

        private void Update(ConsoleExecuteTestStep stored, ConsoleExecuteTestStep changed)
        {
            var shouldPatch = Update(stored as TestStep, changed as TestStep);
            stored.ExecutableFilePath = changed.ExecutableFilePath;

            if (shouldPatch)
            {
                stored.IsPatched = false;
                Patch(stored);
            }
        }

        private void Update(MsiInstallTestStep stored, MsiInstallTestStep changed)
        {
            var shouldPatch = Update(stored as TestStep, changed as TestStep);
            if (shouldPatch)
            {
                stored.IsPatched = false;
                Patch(stored);
            }
        }

        private void Update(ScriptExecuteTestStep stored, ScriptExecuteTestStep changed)
        {
            var shouldPatch = Update(stored as TestStep, changed as TestStep);
            stored.Language = changed.Language;
            
            if (shouldPatch)
            {
                stored.IsPatched = false;
                Patch(stored);
            }
        }

        private void Update(XCopyTestStep stored, XCopyTestStep changed)
        {
            var shouldPatch = Update(stored as TestStep, changed as TestStep);
            stored.Destination = changed.Destination;
            
            if (shouldPatch)
            {
                stored.IsPatched = false;
                Patch(stored);
            }
        }

        /// <summary>
        /// Deletes an existing test step.
        /// </summary>
        /// <param name="id">The ID of the test step.</param>
        public void DeleteTestStep(int id)
        {
            VerifySchemaVersion();

            var testStep = TestStep(id);
            if (testStep != null)
            {
                StoredTestSteps.Remove(testStep);
            }
        }

        /// <summary>
        /// Adds a new test step parameter.
        /// </summary>
        /// <param name="parameter">The new parameter.</param>
        public void Add(TestStepParameter parameter)
        {
            VerifySchemaVersion();

            var result = StoredTestStepParameters.Add(parameter);
            Patch(result);
        }

        /// <summary>
        /// Updates the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter that should be updated.</param>
        public void Update(TestStepParameter parameter)
        {
            VerifySchemaVersion();

            var storedParameter = TestStepParameter(parameter.Id);
            if (storedParameter != null)
            {
                if (!ReferenceEquals(storedParameter, parameter))
                {
                    storedParameter.Key = parameter.Key;
                    storedParameter.Value = parameter.Value;

                    if (storedParameter.fk_TestStepId != parameter.fk_TestStepId)
                    {
                        storedParameter.fk_TestStepId = parameter.fk_TestStepId;
                        storedParameter.TestStep = null;
                        Patch(storedParameter);
                    }
                }

                var entry = Entry(storedParameter);
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes an existing test step parameter.
        /// </summary>
        /// <param name="id">The ID of the parameter.</param>
        public void DeleteTestStepParameter(int id)
        {
            VerifySchemaVersion();

            var parameter = TestStepParameter(id);
            if (parameter != null)
            {
                StoredTestStepParameters.Remove(parameter);
            }
        }

        /// <summary>
        /// Adds a new test application.
        /// </summary>
        /// <param name="application">The new test application.</param>
        public void Add(TestApplication application)
        {
            VerifySchemaVersion();

            var result = StoredTestApplications.Add(application);
            Patch(result);
        }

        /// <summary>
        /// Updates the given application.
        /// </summary>
        /// <param name="application">The test application that should be updated.</param>
        public void Update(TestApplication application)
        {
            VerifySchemaVersion();

            var storedApplication = TestApplication(application.Id);
            if (storedApplication != null)
            {
                if (!ReferenceEquals(storedApplication, application))
                {
                    var shouldPatch = false;
                    if (storedApplication.fk_ApplicationId != application.fk_ApplicationId)
                    {
                        storedApplication.fk_ApplicationId = application.fk_ApplicationId;
                        storedApplication.Application = null;
                        shouldPatch = true;
                    }

                    if (storedApplication.fk_TestEnvironmentId != application.fk_TestEnvironmentId)
                    {
                        storedApplication.fk_TestEnvironmentId = application.fk_TestEnvironmentId;
                        storedApplication.TestEnvironment = null;
                        shouldPatch = true;
                    }

                    if (shouldPatch)
                    {
                        Patch(storedApplication);
                    }
                }

                var entry = Entry(storedApplication);
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Deletes an existing test application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        public void DeleteTestApplication(int id)
        {
            VerifySchemaVersion();

            var application = TestApplication(id);
            if (application != null)
            {
                StoredTestApplications.Remove(application);
            }
        }
    }
}
