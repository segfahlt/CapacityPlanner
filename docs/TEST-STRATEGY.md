# Test Strategy

This document describes how we test the Capacity Planner using TDD.

Guiding principles
- Test from the outside-in by feature. Thin endpoints/controllers; business logic in services.
- Make small, incremental commits: write a failing test, implement minimal code, make it pass, refactor.
- Keep unit tests fast and deterministic. Use Testcontainers for integration and API tests.

Test types
- Unit tests (no I/O):
  - Target: pure functions, calculators, small service methods where persistence is mocked.
  - Tools: xUnit, FluentAssertions.
- Integration tests (EF + SQL Server):
  - Target: service behaviors that depend on EF model and DB constraints.
  - Tools: DotNet.Testcontainers + `mcr.microsoft.com/mssql/server:2022-latest`.
  - DB bootstrapping: run `Persist.CapacityPlanner/RawDbFiles/Tables/Tables.build.sql` or migrations.
- API tests:
  - Target: minimal API endpoints contract and composition (DI, validation, status codes).
  - Tools: WebApplicationFactory + Testcontainers SQL. Override `ConnectionStrings:CapacityPlanner` in test host.
- E2E/UI (later):
  - Target: Blazor flows using MudBlazor components.
  - Tools: Playwright for .NET.

Structure
- `Services.CapacityPlanner.Tests` – service tests grouped by feature directory.
- `CapacityPlanner.Api.Tests` – endpoint tests grouped by feature directory.
- Naming: `Given_When_Then` as test method names.
- Collections/fixtures: share a single SQL container per test collection (`[Collection("sql-server")]`).

Data management
- Prefer short-lived data created in test setup, deleted on teardown by transaction scope or left isolated per test DB.
- Use deterministic builders for entities to reduce duplication.

CI considerations
- Run unit tests on every build.
- Run integration/API tests on PR. Cache SQL Server container image between runs if possible.

Coverage goals
- Core CRUD behaviors for each feature (list/get/create/update/delete).
- Constraint behaviors (unique indexes/FKs).
- Model runner invariants and calculations.

How to write a new test
1) Choose feature and layer (service/api).
2) Add a failing test that describes the behavior and expected outcome.
3) Implement minimal code in the services layer to make it pass.
4) Refactor and add edge case tests.

Quality gates
- All tests pass.
- No flakiness: each test must be deterministic.
- Keep test runtime reasonable (unit tests < 100ms, integration under a few seconds per suite).
