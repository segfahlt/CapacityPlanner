using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;
using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Tests.People;

[Collection("sql-server")]
public class PersonSkillServiceTests
{
    private readonly SqlServerContainerFixture _fixture;
    public PersonSkillServiceTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    private IPersonSkillService CreateSvc(out CapacityPlannerContext ctx)
    {
        var options = _fixture.CreateOptions();
        ctx = new CapacityPlannerContext(options);
        return new Services.CapacityPlanner.PersonSkillService(ctx);
    }

    [Fact]
    public async Task Create_Succeeds_And_Persists()
    {
        var svc = CreateSvc(out var ctx);
        var personId = await ctx.Person.Select(p => p.PersonId).FirstAsync();
        var skillId = await ctx.Skill.Select(s => s.SkillId).FirstAsync();

        var id = await svc.CreateAsync(new PersonSkillDto { PersonId = personId, SkillId = skillId, ProficiencyLevel = "Advanced", Notes = "notes" });

        var exists = await ctx.PersonSkill.AsNoTracking().AnyAsync(ps => ps.PersonSkillId == id);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Create_Duplicate_Throws()
    {
        var svc = CreateSvc(out var ctx);
        var personId = await ctx.Person.Select(p => p.PersonId).FirstAsync();
        var skillId = await ctx.Skill.Select(s => s.SkillId).FirstAsync();

        await svc.CreateAsync(new PersonSkillDto { PersonId = personId, SkillId = skillId });
        Func<Task> act = () => svc.CreateAsync(new PersonSkillDto { PersonId = personId, SkillId = skillId });
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Update_ChangesValues()
    {
        var svc = CreateSvc(out var ctx);
        var personId = await ctx.Person.Select(p => p.PersonId).FirstAsync();
        var skillId = await ctx.Skill.Select(s => s.SkillId).FirstAsync();
        var id = await svc.CreateAsync(new PersonSkillDto { PersonId = personId, SkillId = skillId, ProficiencyLevel = "Beginner", Notes = "a" });

        var ok = await svc.UpdateAsync(new PersonSkillDto { PersonSkillId = id, ProficiencyLevel = "Expert", Notes = "b" });
        ok.Should().BeTrue();

        var updated = await ctx.PersonSkill.AsNoTracking().FirstAsync(ps => ps.PersonSkillId == id);
        updated.ProficiencyLevel.Should().Be("Expert");
        updated.Notes.Should().Be("b");
    }

    [Fact]
    public async Task Delete_RemovesRow()
    {
        var svc = CreateSvc(out var ctx);
        var personId = await ctx.Person.Select(p => p.PersonId).FirstAsync();
        var skillId = await ctx.Skill.Select(s => s.SkillId).FirstAsync();
        var id = await svc.CreateAsync(new PersonSkillDto { PersonId = personId, SkillId = skillId });

        var ok = await svc.DeleteAsync(id);
        ok.Should().BeTrue();

        var exists = await ctx.PersonSkill.AsNoTracking().AnyAsync(ps => ps.PersonSkillId == id);
        exists.Should().BeFalse();
    }
}
