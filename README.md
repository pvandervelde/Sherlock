# Sherlock

Sherlock provides a means to schedule and automatically execute tests in an controlled environment. In order to schedule a test the configuration and the test data can be uploaded to the server via a console application. The test configuration describes all the steps necessary to perform a complete test. This includes installation of MSI files, copying files / directories and execution of powershell scripts. 

For environments Sherlock uses Hyper-V virtual machines which allow easy control of the environment state prior to the execution of a test.

Sherlock consists of: 

* __Sherlock.Console__ - The command line application that can be used to schedule tests based on a XML configuration file. The application extracts the desired information from the configuration file and uploads the test information and the required binaries, e.g. MSI installer files, to the server.
* __Sherlock.Service.Master__ - The application that schedules and tracks the execution of the tests.
* __Sherlock.Service.Executor__ - The application that runs on the test environment which handles the communication with the _Sherlock.Service.Master_ service for exchange of test information, test files and test reports.
* __Sherlock.Executor__ - The application that executes the actual test steps.
* __Sherlock.Web.Api__ - The web service that is used to register tests. 
* __Sherlock.Web.Intranet__ - Provides a way to add, update and remove information about the available test environments. Can also be used to temporarily disable an environment, e.g. when maintenance needs to be done on the environment.
* __Sherlock.Service__ - The service that controls the _Sherlock.Service.Master_ and _Sherlock.Service.Executor_ applications and allows automatic updates when new versions become available.


The [installation](https://github.com/pvandervelde/Sherlock/wiki/Installation) and [operating](https://github.com/pvandervelde/Sherlock/wiki/Using) procedures are described on the wiki.

# How to build
The solution files are created in Visual Studio 2012 (using .NET 4.5) and the entire project can be build by invoking MsBuild on the sherlock.integration.msbuild script. This will build the binaries and the ZIP archives. The binaries will be placed in the `build\bin\AnyCpu\Release` directory and the ZIP archives will be placed in the `build\deploy` directory. Note that the two web projects are an exception in that they place their binaries inside the bin directory of the web project.

Note that the build scripts assume that:

* The binaries should be signed, however the SNK key file is not included in the repository so a new key file has to be [created][snkfile_msdn]. The key file is referenced through an environment variable called `SOFTWARE_SIGNING_KEY_PATH` that has as value the full path of the key file. 
* GIT can be found on the PATH somewhere so that it can be called to get the hash of the last commit in the current repository. This hash is embedded in the Sherlock executable together with information about the build configuration and build time and date.

The icon for Sherlock is the [User detective icon](http://www.fatcow.com/free-icons) which is licensed under the [CC Attribution 3.0](http://creativecommons.org/licenses/by/3.0/) license.

[snkfile_msdn]: http://msdn.microsoft.com/en-us/library/6f05ezxy(v=vs.110).aspx