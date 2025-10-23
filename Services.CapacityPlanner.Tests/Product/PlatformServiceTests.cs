using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;
using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Tests.Product;

[Collection("sql-server")]
public class PlatformServiceTests
{
    private readonly SqlServerContainerFixture _fixture;
    public PlatformServiceTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    private IPlatformService CreateSvc(out CapacityPlannerContext ctx)
    {
        var options = _fixture.CreateOptions();
        ctx = new CapacityPlannerContext(options);
        return new Services.CapacityPlanner.PlatformService(ctx);
    }

    [Fact]
    public async Task Create_And_Update()
    {
        var svc = CreateSvc(out var ctx);
        var id = await svc.CreateAsync(new PlatformDto { Name = "AutoTest Platform A", Description = "desc" });
        var ok = await svc.UpdateAsync(new PlatformDto { PlatformId = id, Name = "AutoTest Platform A2", Description = "d2" });
        ok.Should().BeTrue();
        var updated = await ctx.Platform.AsNoTracking().FirstAsync(p => p.PlatformId == id);
        updated.Name.Should().Be("AutoTest Platform A2");
    }

    [Fact]
    public async Task DuplicateName_Throws()
    {
        var svc = CreateSvc(out _);
        await svc.CreateAsync(new PlatformDto { Name = "AutoTest Platform B" });
        Func<Task> act = () => svc.CreateAsync(new PlatformDto { Name = "AutoTest Platform B" });
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
