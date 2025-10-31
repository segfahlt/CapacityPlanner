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

        // Admin area (scaffold)
        var admin = api.MapGroup("/admin");
        // TODO: Add authorization policy once auth is wired up, e.g., RequireAuthorization("Admin")
        admin.MapGet("", () => Results.Ok(new { status = "ok" }));
        admin.MapGet("/summary", async (CapacityPlannerContext db, CancellationToken ct) =>
        {
            var summary = new
            {
                Platforms = await db.Platform.CountAsync(ct),
                Modules = await db.Module.CountAsync(ct),
                People = await db.Person.CountAsync(ct),
                Roles = await db.Role.CountAsync(ct),
                Skills = await db.Skill.CountAsync(ct),
                Implementations = await db.Implementation.CountAsync(ct)
            };
            return Results.Ok(summary);
        });

        // Lookups (CRUD per type)
        var lookups = api.MapGroup("/lookups");
        lookups.MapGet("/types", () => Results.Ok(new Common.CapacityPlanner.Dto.LookupTypeDto[]
        {
            new("allocation-type","Allocation Types"),
            new("employment-type","Employment Types"),
            new("implementation-category","Implementation Categories"),
            new("status","Statuses"),
            new("workstream-category","Workstream Categories"),
        }));

        lookups.MapGet("/{type}", async (string type, CapacityPlannerContext db, CancellationToken ct) =>
        {
            switch (type.ToLowerInvariant())
            {
                case "allocation-type":
                    return Results.Ok(await db.LookupAllocationType.AsNoTracking().OrderBy(x=>x.AllocationType).Select(x => new Common.CapacityPlanner.Dto.LookupItemDto(x.AllocationType, x.Description)).ToListAsync(ct));
                case "employment-type":
                    return Results.Ok(await db.LookupEmploymentType.AsNoTracking().OrderBy(x=>x.EmploymentType).Select(x => new Common.CapacityPlanner.Dto.LookupItemDto(x.EmploymentType, x.Description)).ToListAsync(ct));
                case "implementation-category":
                    return Results.Ok(await db.LookupImplementationCategory.AsNoTracking().OrderBy(x=>x.ImplementationCategory).Select(x => new Common.CapacityPlanner.Dto.LookupItemDto(x.ImplementationCategory, x.Description)).ToListAsync(ct));
                case "status":
                    return Results.Ok(await db.LookupStatus.AsNoTracking().OrderBy(x=>x.Status).Select(x => new Common.CapacityPlanner.Dto.LookupItemDto(x.Status, x.Description)).ToListAsync(ct));
                case "workstream-category":
                    return Results.Ok(await db.LookupWorkstreamCategory.AsNoTracking().OrderBy(x=>x.WorkstreamCategory).Select(x => new Common.CapacityPlanner.Dto.LookupItemDto(x.WorkstreamCategory, x.Description)).ToListAsync(ct));
                default:
                    return Results.NotFound(new { message = $"Unknown lookup type '{type}'" });
            }
        });

        lookups.MapPost("/{type}", async (string type, Common.CapacityPlanner.Dto.LookupItemDto item, CapacityPlannerContext db, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(item.Key)) return Results.BadRequest(new { message = "Key is required" });
            try
            {
                switch (type.ToLowerInvariant())
                {
                    case "allocation-type":
                        if (await db.LookupAllocationType.AnyAsync(x=>x.AllocationType==item.Key, ct)) return Results.Conflict(new { message = "Key already exists" });
                        db.LookupAllocationType.Add(new LookupAllocationType { AllocationType = item.Key, Description = item.Description });
                        break;
                    case "employment-type":
                        if (await db.LookupEmploymentType.AnyAsync(x=>x.EmploymentType==item.Key, ct)) return Results.Conflict(new { message = "Key already exists" });
                        db.LookupEmploymentType.Add(new LookupEmploymentType { EmploymentType = item.Key, Description = item.Description });
                        break;
                    case "implementation-category":
                        if (await db.LookupImplementationCategory.AnyAsync(x=>x.ImplementationCategory==item.Key, ct)) return Results.Conflict(new { message = "Key already exists" });
                        db.LookupImplementationCategory.Add(new LookupImplementationCategory { ImplementationCategory = item.Key, Description = item.Description });
                        break;
                    case "status":
                        if (await db.LookupStatus.AnyAsync(x=>x.Status==item.Key, ct)) return Results.Conflict(new { message = "Key already exists" });
                        db.LookupStatus.Add(new LookupStatus { Status = item.Key, Description = item.Description });
                        break;
                    case "workstream-category":
                        if (await db.LookupWorkstreamCategory.AnyAsync(x=>x.WorkstreamCategory==item.Key, ct)) return Results.Conflict(new { message = "Key already exists" });
                        db.LookupWorkstreamCategory.Add(new LookupWorkstreamCategory { WorkstreamCategory = item.Key, Description = item.Description });
                        break;
                    default:
                        return Results.NotFound(new { message = $"Unknown lookup type '{type}'" });
                }
                await db.SaveChangesAsync(ct);
                return Results.Created($"/api/lookups/{type}/{item.Key}", item);
            }
            catch (DbUpdateException ex)
            {
                return Results.Conflict(new { message = ex.GetBaseException().Message });
            }
        });

        lookups.MapPut("/{type}/{key}", async (string type, string key, Common.CapacityPlanner.Dto.LookupItemDto update, CapacityPlannerContext db, CancellationToken ct) =>
        {
            try
            {
                switch (type.ToLowerInvariant())
                {
                    case "allocation-type":
                        {
                            var e = await db.LookupAllocationType.FirstOrDefaultAsync(x=>x.AllocationType==key, ct);
                            if (e is null) return Results.NotFound();
                            if (!string.Equals(update.Key, key, StringComparison.Ordinal)) return Results.BadRequest(new { message = "Renaming keys is not supported" });
                            e.Description = update.Description;
                            break;
                        }
                    case "employment-type":
                        {
                            var e = await db.LookupEmploymentType.FirstOrDefaultAsync(x=>x.EmploymentType==key, ct);
                            if (e is null) return Results.NotFound();
                            if (!string.Equals(update.Key, key, StringComparison.Ordinal)) return Results.BadRequest(new { message = "Renaming keys is not supported" });
                            e.Description = update.Description;
                            break;
                        }
                    case "implementation-category":
                        {
                            var e = await db.LookupImplementationCategory.FirstOrDefaultAsync(x=>x.ImplementationCategory==key, ct);
                            if (e is null) return Results.NotFound();
                            if (!string.Equals(update.Key, key, StringComparison.Ordinal)) return Results.BadRequest(new { message = "Renaming keys is not supported" });
                            e.Description = update.Description;
                            break;
                        }
                    case "status":
                        {
                            var e = await db.LookupStatus.FirstOrDefaultAsync(x=>x.Status==key, ct);
                            if (e is null) return Results.NotFound();
                            if (!string.Equals(update.Key, key, StringComparison.Ordinal)) return Results.BadRequest(new { message = "Renaming keys is not supported" });
                            e.Description = update.Description;
                            break;
                        }
                    case "workstream-category":
                        {
                            var e = await db.LookupWorkstreamCategory.FirstOrDefaultAsync(x=>x.WorkstreamCategory==key, ct);
                            if (e is null) return Results.NotFound();
                            if (!string.Equals(update.Key, key, StringComparison.Ordinal)) return Results.BadRequest(new { message = "Renaming keys is not supported" });
                            e.Description = update.Description;
                            break;
                        }
                    default:
                        return Results.NotFound(new { message = $"Unknown lookup type '{type}'" });
                }
                await db.SaveChangesAsync(ct);
                return Results.NoContent();
            }
            catch (DbUpdateException ex)
            {
                return Results.Conflict(new { message = ex.GetBaseException().Message });
            }
        });

        lookups.MapDelete("/{type}/{key}", async (string type, string key, CapacityPlannerContext db, CancellationToken ct) =>
        {
            try
            {
                switch (type.ToLowerInvariant())
                {
                    case "allocation-type":
                        {
                            var e = await db.LookupAllocationType.FirstOrDefaultAsync(x=>x.AllocationType==key, ct);
                            if (e is null) return Results.NotFound();
                            db.LookupAllocationType.Remove(e);
                            break;
                        }
                    case "employment-type":
                        {
                            var e = await db.LookupEmploymentType.FirstOrDefaultAsync(x=>x.EmploymentType==key, ct);
                            if (e is null) return Results.NotFound();
                            db.LookupEmploymentType.Remove(e);
                            break;
                        }
                    case "implementation-category":
                        {
                            var e = await db.LookupImplementationCategory.FirstOrDefaultAsync(x=>x.ImplementationCategory==key, ct);
                            if (e is null) return Results.NotFound();
                            db.LookupImplementationCategory.Remove(e);
                            break;
                        }
                    case "status":
                        {
                            var e = await db.LookupStatus.FirstOrDefaultAsync(x=>x.Status==key, ct);
                            if (e is null) return Results.NotFound();
                            db.LookupStatus.Remove(e);
                            break;
                        }
                    case "workstream-category":
                        {
                            var e = await db.LookupWorkstreamCategory.FirstOrDefaultAsync(x=>x.WorkstreamCategory==key, ct);
                            if (e is null) return Results.NotFound();
                            db.LookupWorkstreamCategory.Remove(e);
                            break;
                        }
                    default:
                        return Results.NotFound(new { message = $"Unknown lookup type '{type}'" });
                }
                await db.SaveChangesAsync(ct);
                return Results.NoContent();
            }
            catch (DbUpdateException ex)
            {
                return Results.Conflict(new { message = ex.GetBaseException().Message });
            }
        });

        // Platforms API (for TreeNav + CRUD for Admin)
        api.MapGet("/platforms", async (IPlatformService svc, CancellationToken ct) => Results.Ok(await svc.ListAsync(ct)));
        api.MapPost("/platforms", async (Common.CapacityPlanner.Dto.PlatformDto dto, IPlatformService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(dto, ct);
                return Results.Created($"/api/platforms/{id}", new { id });
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
        api.MapPut("/platforms/{id:guid}", async (Guid id, Common.CapacityPlanner.Dto.PlatformDto dto, IPlatformService svc, CancellationToken ct) =>
        {
            try
            {
                dto.PlatformId = id;
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
        api.MapDelete("/platforms/{id:guid}", async (Guid id, IPlatformService svc, CancellationToken ct) => (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

        // Modules API filtered by platformId + CRUD
        api.MapGet("/modules", async (Guid platformId, IModuleService svc, CancellationToken ct) =>
        {
            var mods = await svc.ListAsync(ct);
            return Results.Ok(mods.Where(m => m.PlatformId == platformId));
        });
        api.MapPost("/modules", async (Common.CapacityPlanner.Dto.ModuleDto dto, IModuleService svc, CancellationToken ct) =>
        {
            try
            {
                var id = await svc.CreateAsync(dto, ct);
                return Results.Created($"/api/modules/{id}", new { id });
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
        api.MapPut("/modules/{id:guid}", async (Guid id, Common.CapacityPlanner.Dto.ModuleDto dto, IModuleService svc, CancellationToken ct) =>
        {
            try
            {
                dto.ModuleId = id;
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
        api.MapDelete("/modules/{id:guid}", async (Guid id, IModuleService svc, CancellationToken ct) => (await svc.DeleteAsync(id, ct)) ? Results.NoContent() : Results.NotFound());

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
