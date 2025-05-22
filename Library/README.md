# How to Run This Project

This application has been split into three containers:
- Library.API
- Library.Reporting.Service
- mssqlserver-2022

To run the project, navigate to the `src` folder and run:
```bash
docker compose up
```
Once the containers are up, the APIs will be accessible at [http://localhost:8080](http://localhost:8080). You can also access the Swagger (Open API) UI at [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html).

There are two Docker Compose files:
- **docker-compose.yml**: Runs all three containers and sets up the database in release mode.
- **docker-compose.debug.yml**: Runs the containers in debug mode with `vsdbg` attached for debugging. (Predefined VS Code tasks are available in `tasks.json`.)

# Tests

The project contains two categories of tests: functional and integration tests. You can run all tests at once or only tests from a specific category.

To run all tests:
```bash
dotnet test
```

To run only functional tests:
```bash
dotnet test --filter "Category=FunctionalTests"
```

To run only integration tests:
```bash
dotnet test --filter "Category=IntegrationTests"
```

The tests are independent of the main application containers. They use a NuGet package called `TestContainers` to set up their own environment. Ensure that Docker Desktop is running before executing the tests.

Currently, a new database container is launched for per integration test to ensure a clean instance. Although not optimal, this approach prevents race conditions and flaky behavior. Note that running tests may take some time and consume additional memory temporarily.

# Database

You can connect directly to the database using `localhost,1433`. The `Library.Reporting.Service` project contains EF Core migration code that automatically creates and seeds the database when the MSSQL server container is running.

In tests, the creation and seeding of data is performed on a per-test basis as described in the **Tests** section above. Each test launches a new database container, ensuring a clean instance to avoid race conditions and flaky behavior. This process may take some time and temporarily use extra memory.
