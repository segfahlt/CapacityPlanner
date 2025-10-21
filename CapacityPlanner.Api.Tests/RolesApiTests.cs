using System.Net;
using FluentAssertions;

namespace CapacityPlanner.Api.Tests;

public class RolesApiTests : IClassFixture<Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>>
{
    private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory;
    public RolesApiTests(Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Get_Roles_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/roles");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
