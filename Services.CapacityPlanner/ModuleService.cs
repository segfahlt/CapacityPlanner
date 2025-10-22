using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;

namespace Services.CapacityPlanner;

public sealed class ModuleService : IModuleService
{
    private readonly CapacityPlannerContext _db;
    public ModuleService(CapacityPlannerContext db) => _db = db;

    public Task<List<Module>> ListAsync(CancellationToken ct = default) =>
        _db.Module.AsNoTracking().OrderBy(m => m.Name).ToListAsync(ct);

    public Task<Module?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Module.AsNoTracking().FirstOrDefaultAsync(m => m.ModuleId == id, ct);

    public async Task<Guid> CreateAsync(Guid platformId, string name, string? description, string? status, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        var platformExists = await _db.Platform.AsNoTracking().AnyAsync(p => p.PlatformId == platformId, ct);
        if (!platformExists) throw new InvalidOperationException("Platform not found");
        var taken = await _db.Module.AsNoTracking().AnyAsync(m => m.PlatformId == platformId && m.Name == name, ct);
        if (taken) throw new InvalidOperationException("Module name must be unique per platform");
        var entity = new Module { ModuleId = Guid.NewGuid(), PlatformId = platformId, Name = name, Description = description, Status = status };
        _db.Module.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.ModuleId;
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string? description, string? status, CancellationToken ct = default)
    {
        var entity = await _db.Module.FirstOrDefaultAsync(m => m.ModuleId == id, ct);
        if (entity is null) return false;
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        var taken = await _db.Module.AsNoTracking().AnyAsync(m => m.PlatformId == entity.PlatformId && m.Name == name && m.ModuleId != id, ct);
        if (taken) throw new InvalidOperationException("Module name must be unique per platform");
        entity.Name = name;
        entity.Description = description;
        entity.Status = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Module.FirstOrDefaultAsync(m => m.ModuleId == id, ct);
        if (entity is null) return false;
        _db.Module.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
