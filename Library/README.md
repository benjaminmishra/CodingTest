# HOW TO RUN THIS PROJECT

This application has been split into three containers i.e.

- Library.API
- Library.Reporting.Service
- mssqlserver-2022

So to run this application it is suffcient to go into src folder and run `docker compose up` . Once that is done the APIs should be accessible on localhost:80.
Also you can open the swagger (open api) UI via http://localhost/swagger/index.html.

There are two docker compose files

- docker-compose.yml - Runs all three containers and sets up the database in release mode.
- docker-compose.debug.yml -  Can be used to run the containers in debug mode and attachd vsdbg to them for debugging. (There are predefined vs code tasks in tasks.json for this)


# TESTS

Tests are independent of the above mentioned containers. The tests use a nuget package called `TestContainers` to setup their own environment. So you can just run your docker desktop and then call `dotnet test` in the tests folder. Its important to note that docker desktop needs to be up and running before you run the test.

Right now this project launches a new database (as a container) for every test. That is however not very optimal but for now it works as we get a clean database instace for each test. This is important to make sure there are no race contions and flaky behavior for now. Hence, running tests might require a little bit of time and might hog some memory for a short period of time.
