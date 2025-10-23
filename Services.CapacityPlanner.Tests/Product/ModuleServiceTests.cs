using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;
using Common.CapacityPlanner.Dto;

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
        var id = await svc.CreateAsync(new ModuleDto { PlatformId = platformId, Name = "AutoTest Module A", Description = "desc", Status = "Active" });
        var ok = await svc.UpdateAsync(new ModuleDto { ModuleId = id, PlatformId = platformId, Name = "AutoTest Module A2", Description = "d2", Status = "Planned" });
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
        await svc.CreateAsync(new ModuleDto { PlatformId = platformId, Name = "AutoTest Module B" });
        Func<Task> act = () => svc.CreateAsync(new ModuleDto { PlatformId = platformId, Name = "AutoTest Module B" });
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
