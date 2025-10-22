using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;
using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Tests.Roles;

[Collection("sql-server")]
public class RoleServiceTests
{
    private readonly SqlServerContainerFixture _fixture;
    public RoleServiceTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    private IRoleService CreateSvc()
    {
        var options = _fixture.CreateOptions();
        var db = new CapacityPlannerContext(options);
        return new Services.CapacityPlanner.RoleService(db);
    }

    [Fact]
    public async Task Create_Succeeds_And_Persists()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new RoleDto { Name = "AutoTest Planner", DefaultUtilizationTarget = 0.85m });
        var list = await svc.ListAsync();
        list.Should().Contain(x => x.RoleId == id && x.Name == "AutoTest Planner");
    }

    [Fact]
    public async Task Create_DuplicateName_Throws()
    {
        var svc = CreateSvc();
        await svc.CreateAsync(new RoleDto { Name = "AutoTest Dev", DefaultUtilizationTarget = 0.85m });
        Func<Task> act = () => svc.CreateAsync(new RoleDto { Name = "AutoTest Dev", DefaultUtilizationTarget = 0.9m });
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Update_ChangesValues()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new RoleDto { Name = "AutoTest QA", DefaultUtilizationTarget = 0.85m });
        var ok = await svc.UpdateAsync(new RoleDto { RoleId = id, Name = "AutoTest QA Engineer", DefaultUtilizationTarget = 0.8m });
        ok.Should().BeTrue();

        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var updated = await ctx.Role.AsNoTracking().FirstAsync(r => r.RoleId == id);
        updated.Name.Should().Be("AutoTest QA Engineer");
        updated.DefaultUtilizationTarget.Should().Be(0.8m);
    }

    [Fact]
    public async Task Delete_RemovesRow()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new RoleDto { Name = "AutoTest TempRole", DefaultUtilizationTarget = 0.5m });
        var ok = await svc.DeleteAsync(id);
        ok.Should().BeTrue();

        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var exists = await ctx.Role.AsNoTracking().AnyAsync(r => r.RoleId == id);
        exists.Should().BeFalse();
    }
}
