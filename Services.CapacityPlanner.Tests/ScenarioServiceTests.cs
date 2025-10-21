using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Tests.Infrastructure;

namespace Services.CapacityPlanner.Tests;

[Collection("sql-server")]
public class ScenarioServiceTests
{
    private readonly SqlServerContainerFixture _fixture;

    public ScenarioServiceTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateScenario_Persists()
    {
        // Arrange
        var options = _fixture.CreateOptions();
        await using var db = new CapacityPlannerContext(options);
        var svc = new ScenarioService(db);

        // Act
        var id = await svc.CreateScenarioAsync("Test Scenario", "desc", null);

        // Assert
        var exists = await db.Scenario.AsNoTracking().AnyAsync(s => s.ScenarioId == id);
        exists.Should().BeTrue();
    }
}
