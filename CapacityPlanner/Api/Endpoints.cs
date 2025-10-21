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
        roles.MapGet("", async (CapacityPlannerContext db, CancellationToken ct) =>
            await db.Role.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct));
        roles.MapGet("/{id:guid}", async (Guid id, CapacityPlannerContext db, CancellationToken ct) =>
            await db.Role.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == id, ct) is { } r
                ? Results.Ok(r)
                : Results.NotFound());
        roles.MapPost("", async (Role role, CapacityPlannerContext db, CancellationToken ct) =>
        {
            db.Role.Add(role);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/roles/{role.RoleId}", role);
        });
        roles.MapPut("/{id:guid}", async (Guid id, Role update, CapacityPlannerContext db, CancellationToken ct) =>
        {
            var existing = await db.Role.FindAsync([id], ct);
            if (existing is null) return Results.NotFound();
            existing.Name = update.Name;
            existing.DefaultUtilizationTarget = update.DefaultUtilizationTarget;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });
        roles.MapDelete("/{id:guid}", async (Guid id, CapacityPlannerContext db, CancellationToken ct) =>
        {
            var existing = await db.Role.FindAsync([id], ct);
            if (existing is null) return Results.NotFound();
            db.Role.Remove(existing);
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

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
