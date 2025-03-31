# The Game of Life

An implementation of the [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life). It implements a core engine with persistence support through a REST API.

## Running the application

Check the `docker-compose.yaml` to learn how the solution is deployed. The easiest way to run the solution is through the following command:

> podman compose -f docker-compose.yaml up --build app

By default, the application will listen on port `8080`. Launch `http://localhost:8080/swagger`.

## Architecture & Engineering highlights

The following list highlights the principal decisions:

* Software Architecture: simple because the application is very simple (_that is just a code challenge_) but with clear separation of concerns. Principal logical divisions ("modules" or namespaces)
    *  _Core_: implementation of the business logic. Should be agnostic about infrastructure or presentation tiers and uses _Rich Domain Model_ principles (in short, _DDD_ and strong _OOP_)
    * _Persistence_: wraps the core objects to be persisted in databases like MongoDB
    * _Presentation_: implements the REST API, and that is both the application (e.g. use cases, model conversions) and service (REST API) tiers. This model was choose because of it simplicity and efficiency for the problem
* Persistence in MongoDB: the solution just needs a practical and scalable place to store the state to not lose data (crashes and so on). MongoDB attends the requirement with ease and there is no requirement for transactional processing (ACID, relational databases).
* Keep in mind: well-balancing of design and performance by understanding .NET fundamentals (e.g. _class vs struct_, allocations, and so on)

And the following list highlight some improvements:

* Stereotype for Core's objects (`Entity`, `ValueObject`, `AggregateRoot`)
* Load and stress tests (pending) - for example, using K6 and Visual Studio Profiler (or similar)
* Use secrets (`docker secrets`) or more robust Key Vault solution instead of _environment variables_
* Improve documentation and remove all warnings (and set `WarningAsError=true`)
* Add Roslyn analyzers such as .NET CodeAnalysis, [integrated Sonarqube](https://www.nuget.org/packages/SonarAnalyzer.CSharp), and [CLR Heap Allocation Analyzer](https://www.nuget.org/packages/ClrHeapAllocationAnalyzer)
