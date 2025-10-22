using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;

namespace Services.CapacityPlanner.Tests.Product;

[Collection("sql-server")]
public class ModuleServiceTests
{
    private readonly SqlServerContainerFixture _fixture;
    public ModuleServiceTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    private (IModuleService svc, CapacityPlannerContext ctx) CreateSvc()
    {
        var options = _fixture.CreateOptions();
        var ctx = new CapacityPlannerContext(options);
        return (new Services.CapacityPlanner.ModuleService(ctx), ctx);
    }

    [Fact]
    public async Task Create_And_Update()
    {
        var (svc, ctx) = CreateSvc();
        var platformId = await ctx.Platform.Select(p => p.PlatformId).FirstAsync();
        var id = await svc.CreateAsync(platformId, "AutoTest Module A", "desc", "Active");
        var ok = await svc.UpdateAsync(id, "AutoTest Module A2", "d2", "Planned");
        ok.Should().BeTrue();

        var updated = await ctx.Module.AsNoTracking().FirstAsync(m => m.ModuleId == id);
        updated.Name.Should().Be("AutoTest Module A2");
        updated.Status.Should().Be("Planned");
    }

    [Fact]
    public async Task DuplicateNamePerPlatform_Throws()
    {
        var (svc, ctx) = CreateSvc();
        var platformId = await ctx.Platform.Select(p => p.PlatformId).FirstAsync();
        await svc.CreateAsync(platformId, "AutoTest Module B", null, null);
        Func<Task> act = () => svc.CreateAsync(platformId, "AutoTest Module B", null, null);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
