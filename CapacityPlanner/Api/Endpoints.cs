using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;

namespace CapacityPlanner.Api;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCapacityPlannerApi(this IEndpointRouteBuilder routes)
    {
        var api = routes.MapGroup("/api");

        // Roles
        var roles = api.MapGroup("/roles");
        roles.MapGet("", async (Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
            Results.Ok(await svc.ListAsync(ct)));
        roles.MapGet("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
            await svc.GetAsync(id, ct) is { } r ? Results.Ok(r) : Results.NotFound());
        roles.MapPost("", async (Role role, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(role.Name!, role.DefaultUtilizationTarget, ct);
                return Results.Created($"/api/roles/{id}", new { id });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
        roles.MapPut("/{id:guid}", async (Guid id, Role update, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
        {
            try
            {
                var ok = await svc.UpdateAsync(id, update.Name!, update.DefaultUtilizationTarget, ct);
                return ok ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
        roles.MapDelete("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

        // Skills
        var skills = api.MapGroup("/skills");
        skills.MapGet("", async (CapacityPlannerContext db, CancellationToken ct) =>
            await db.Skill.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct));
        skills.MapPost("", async (Skill skill, CapacityPlannerContext db, CancellationToken ct) =>
        {
            db.Skill.Add(skill);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/skills/{skill.SkillId}", skill);
        });

        // Scenarios
        var scenarios = api.MapGroup("/scenarios");
        scenarios.MapGet("", async (CapacityPlannerContext db, CancellationToken ct) =>
            await db.Scenario.AsNoTracking().OrderByDescending(s => s.CreatedOn).ToListAsync(ct));
        scenarios.MapPost("", async (Scenario scenario, CapacityPlannerContext db, CancellationToken ct) =>
        {
            db.Scenario.Add(scenario);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/scenarios/{scenario.ScenarioId}", scenario);
        });

        return routes;
    }
}
