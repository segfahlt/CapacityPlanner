using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;
using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Tests.Skills;

[Collection("sql-server")]
public class SkillServiceTests
{
    private readonly SqlServerContainerFixture _fixture;
    public SkillServiceTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    private ISkillService CreateSvc()
    {
        var options = _fixture.CreateOptions();
        var db = new CapacityPlannerContext(options);
        return new Services.CapacityPlanner.SkillService(db);
    }

    [Fact]
    public async Task Create_Succeeds_And_Persists()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new SkillDto { Name = "AutoTest Skill A", Description = "desc" });
        var list = await svc.ListAsync();
        list.Should().Contain(x => x.SkillId == id && x.Name == "AutoTest Skill A");
    }

    [Fact]
    public async Task Create_DuplicateName_Throws()
    {
        var svc = CreateSvc();
        await svc.CreateAsync(new SkillDto { Name = "AutoTest Skill B" });
        Func<Task> act = () => svc.CreateAsync(new SkillDto { Name = "AutoTest Skill B" });
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Update_ChangesValues()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new SkillDto { Name = "AutoTest Skill C", Description = "1" });
        var ok = await svc.UpdateAsync(new SkillDto { SkillId = id, Name = "AutoTest Skill C2", Description = "2" });
        ok.Should().BeTrue();

        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var updated = await ctx.Skill.AsNoTracking().FirstAsync(r => r.SkillId == id);
        updated.Name.Should().Be("AutoTest Skill C2");
        updated.Description.Should().Be("2");
    }

    [Fact]
    public async Task Delete_RemovesRow()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new SkillDto { Name = "AutoTest Skill D" });
        var ok = await svc.DeleteAsync(id);
        ok.Should().BeTrue();

        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var exists = await ctx.Skill.AsNoTracking().AnyAsync(r => r.SkillId == id);
        exists.Should().BeFalse();
    }
}
