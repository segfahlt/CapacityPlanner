using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;

namespace CapacityPlanner.Api;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCapacityPlannerApi(this IEndpointRouteBuilder routes)
    {
        var api = routes.MapGroup("/api");

        // Platforms API (for TreeNav)
        api.MapGet("/platforms", async (IPlatformService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));

        // Modules API filtered by platformId
        api.MapGet("/modules", async (Guid platformId, IModuleService svc, CancellationToken ct) =>
        {
            var mods = await svc.ListAsync(ct);
            return Results.Ok(mods.Where(m => m.PlatformId == platformId));
        });

        // Implementations API filtered by moduleId + CRUD
        api.MapGet("/implementations", async (Guid moduleId, IImplementationService svc, CancellationToken ct) =>
        {
            var impls = await svc.ListAsync(moduleId: moduleId, ct: ct);
            return Results.Ok(impls);
        });
        api.MapGet("/implementations/{id:guid}", async (Guid id, IImplementationService svc, CancellationToken ct) =>
        {
            var dto = await svc.GetAsync(id, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        });
        api.MapPost("/implementations", async (Common.CapacityPlanner.Dto.ImplementationDto dto, IImplementationService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(dto, ct);
                return Results.Created($"/api/implementations/{id}", new { id });
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
        api.MapPut("/implementations/{id:guid}", async (Guid id, Common.CapacityPlanner.Dto.ImplementationDto dto, IImplementationService svc, CancellationToken ct) =>
        {
            try
            {
                dto.ImplementationId = id;
                var ok = await svc.UpdateAsync(dto, ct);
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
        api.MapDelete("/implementations/{id:guid}", async (Guid id, IImplementationService svc, CancellationToken ct) => (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

        // existing endpoints ...
        // Roles
        var roles = api.MapGroup("/roles");
        roles.MapGet("", async (Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));
        roles.MapGet("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) => await svc.GetAsync(id, ct) is { } r ? Results.Ok(r) : Results.NotFound());
        roles.MapPost("", async (Common.CapacityPlanner.Dto.RoleDto role, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(role, ct);
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
        roles.MapPut("/{id:guid}", async (Guid id, Common.CapacityPlanner.Dto.RoleDto update, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) =>
        {
            try
            {
                update.RoleId = id;
                var ok = await svc.UpdateAsync(update, ct);
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
        roles.MapDelete("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.IRoleService svc, CancellationToken ct) => (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

        // Skills
        var skills = api.MapGroup("/skills");
        skills.MapGet("", async (Services.CapacityPlanner.Abstraction.ISkillService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));
        skills.MapGet("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.ISkillService svc, CancellationToken ct) => await svc.GetAsync(id, ct) is { } s ? Results.Ok(s) : Results.NotFound());
        skills.MapPost("", async (Common.CapacityPlanner.Dto.SkillDto skill, Services.CapacityPlanner.Abstraction.ISkillService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(skill, ct);
                return Results.Created($"/api/skills/{id}", new { id });
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
        skills.MapPut("/{id:guid}", async (Guid id, Common.CapacityPlanner.Dto.SkillDto update, Services.CapacityPlanner.Abstraction.ISkillService svc, CancellationToken ct) =>
        {
            try
            {
                update.SkillId = id;
                var ok = await svc.UpdateAsync(update, ct);
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
        skills.MapDelete("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.ISkillService svc, CancellationToken ct) => (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

        // Scenarios
        var scenarios = api.MapGroup("/scenarios");
        scenarios.MapGet("", async (CapacityPlannerContext db, CancellationToken ct) => await db.Scenario.AsNoTracking().OrderByDescending(s => s.CreatedOn).ToListAsync(ct));
        scenarios.MapPost("", async (Scenario scenario, CapacityPlannerContext db, CancellationToken ct) =>
        {
            db.Scenario.Add(scenario);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/scenarios/{scenario.ScenarioId}", scenario);
        });

        // People
        var people = api.MapGroup("/person");
        people.MapGet("", async (Services.CapacityPlanner.Abstraction.IPersonService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));
        people.MapGet("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.IPersonService svc, CancellationToken ct) => await svc.GetAsync(id, ct) is { } p ? Results.Ok(p) : Results.NotFound());
        people.MapPost("", async (Common.CapacityPlanner.Dto.PersonDto person, Services.CapacityPlanner.Abstraction.IPersonService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(person, ct);
                return Results.Created($"/api/person/{id}", new { id });
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
        people.MapPut("/{id:guid}", async (Guid id, Common.CapacityPlanner.Dto.PersonDto update, Services.CapacityPlanner.Abstraction.IPersonService svc, CancellationToken ct) =>
        {
            try
            {
                update.PersonId = id;
                var ok = await svc.UpdateAsync(update, ct);
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
        people.MapDelete("/{id:guid}", async (Guid id, Services.CapacityPlanner.Abstraction.IPersonService svc, CancellationToken ct) => (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

        // Implementation Templates
        var templates = api.MapGroup("/implementation-templates");
        templates.MapPost("", async (Services.CapacityPlanner.Contracts.ImplementationTemplateCreateRequest req, Services.CapacityPlanner.Abstraction.IImplementationTemplateService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateTemplateAsync(req, ct);
                return Results.Created($"/api/implementation-templates/{id}", new { id });
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
        templates.MapGet("/{id:guid}", async (Guid id, bool includeGraph, Services.CapacityPlanner.Abstraction.IImplementationTemplateService svc, CancellationToken ct) =>
        {
            var t = await svc.GetTemplateAsync(id, includeGraph, ct);
            return t is null ? Results.NotFound() : Results.Ok(t);
        });

        return routes;
    }
}
