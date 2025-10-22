using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Tests.Infrastructure;
using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Tests.People;

[Collection("sql-server")]
public class PersonServiceTests
{
    private readonly SqlServerContainerFixture _fixture;
    public PersonServiceTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    private IPersonService CreateSvc()
    {
        var options = _fixture.CreateOptions();
        var db = new CapacityPlannerContext(options);
        return new Services.CapacityPlanner.PersonService(db);
    }

    [Fact]
    public async Task Create_Persists()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new PersonDto { Name = "AutoTest Person A", EmploymentType = null, IsActive = true, Location = "US" });
        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var exists = await ctx.Person.AsNoTracking().AnyAsync(p => p.PersonId == id);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Create_WithUnknownEmploymentType_Throws()
    {
        var svc = CreateSvc();
        Func<Task> act = () => svc.CreateAsync(new PersonDto { Name = "AutoTest Person B", EmploymentType = "DOES_NOT_EXIST", IsActive = true });
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Update_ChangesValues()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new PersonDto { Name = "AutoTest Person C", IsActive = true });
        var ok = await svc.UpdateAsync(new PersonDto { PersonId = id, Name = "AutoTest Person C2", IsActive = false, Location = "IN" });
        ok.Should().BeTrue();

        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var updated = await ctx.Person.AsNoTracking().FirstAsync(p => p.PersonId == id);
        updated.Name.Should().Be("AutoTest Person C2");
        updated.IsActive.Should().BeFalse();
        updated.Location.Should().Be("IN");
    }

    [Fact]
    public async Task Delete_Removes()
    {
        var svc = CreateSvc();
        var id = await svc.CreateAsync(new PersonDto { Name = "AutoTest Person D", IsActive = true });
        var ok = await svc.DeleteAsync(id);
        ok.Should().BeTrue();

        var ctx = new CapacityPlannerContext(_fixture.CreateOptions());
        var exists = await ctx.Person.AsNoTracking().AnyAsync(p => p.PersonId == id);
        exists.Should().BeFalse();
    }
}
