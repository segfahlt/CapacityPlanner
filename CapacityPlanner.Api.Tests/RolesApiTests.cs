using System.Net;
using CapacityPlanner.Api.Tests.Infrastructure;
using FluentAssertions;

namespace CapacityPlanner.Api.Tests;

public class RolesApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    public RolesApiTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_Roles_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/roles");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
