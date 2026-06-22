# Overview

- Senswave is Smart Home platform for DIY devices 
- User defines DataSources that send the data to system sytem is responsible for parsing and storing this data
- System allows to create configuratble UI with widget and dashboards
- Operations define Device functions 
- User aggregates Devices in Homes and rooms
- Homes can be shared with different level of permissions - RBAC
- Users can create Automations in home as automatic operation to invoke when something happens

## Code

- using .NET 10
- using Minmal Api
- modular monolit repository 
- corss-cutting concerns are in  - `src/Shared`
- modules with functions are at `src/Modules`
- projects with containers are in - `src/Presentations`
- Senswave can be configured to work with RabbitMq or In Memory Database
- Senswave uses PostgreSQL as database
- Currently External Cache is not used
- Solution Should support HA in the future for main processing parts
- you should consider introducing OpenTelemetry in custom code.

## Development

- always verify build at level of the `Senswave.sln`
- for changes in module at least run test from this module
- use `dotnet build` for building code
- use `dotnet test` for testing code and always run all test in module you make changes
- use to get coverage `dotnet-coverage collect "dotnet test" -f cobertura -o coverage.cobertura.xml`
- use to get coverate report `reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html`

### Development Environment Setup

- use `dotnent dev-certs https -t` to generate and accept certificates for developments
- use `dotnet user-secrets init` to initialize storing secrets
- use `dotnet user-secrets set "Modules:Users:Auth:Google:ClientSecret" "<value>"` for adding secret

### Development Module Conventions

- Module is splited mainly into 4 projects
    - Domain 
        - contains base models like entities, service interfaces, repositories interfaces
        - implements basic business logic for complex modules
    - Application 
        - main bussines logic is implemented here splited into folders with feature names
        - contains service implementations, features handlers using Mediator pattern
        - uses validation based on FluentValidatin
    - Infrastructure 
        - contains database mode definition repositories 
        - ocassional services implementations 
        - contains cross module communication implementation basing on MassTransit
    - Api 
        - REST service definitions
        - dtos models for transporting with extensions for parsing responses
        - uses IMediator to create requests for business flows

### Testing Instructions

- tests are in `test`
- modules tests are in `test/Modules`
- shared logic tests are in `test/Shared`
- Final Api tests are in `test/Presentation`
- tests are splitted by modules
- there are 3 types of tests 
    - `Unit` - testing certain functions with minimal setup
    - `Integration` - testing functionalites requiring containers
    - `End` - API tests for Modules
- code coverage target is `70%`

### API Contract Guidelines

- REST request DTOs (`*Request` classes in `Api` projects) represent a public contract — existing fields must never be renamed or removed, only new fields can be added
- When application commands evolve (e.g. renamed fields), map old DTO field names to new command field names in the `*Extensions` mapping class

### Code style Guidelines

- use `.editorconfig` suggestions

### Security

- don't use new libraries without permission
- don't update libraries by the way

## Integration

### Pull Requests

- if tests fail notify
- don't merge on your own
- merge request tile examples
    - `fix: <info>` - for fixing a bug
    - `feat: <info>` - for new features
    - `chore: <info>` - for maintenance tasks

### Github Workflows

- `dev-tests.yaml` - testing solution, docker build with every pull request
- `staging-tests.yaml` - after merging to main automatically updates staging server
- `prod-worker-deploy.yaml` - builds and deploys worker to production
- `prod-deploy.yaml` - builds and deploys api to production 

### Production Updates

- production updates are created only manually with strict versioning