using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class ModuleService : IModuleService
{
    private readonly CapacityPlannerContext _db;
    public ModuleService(CapacityPlannerContext db) => _db = db;

    public async Task<List<ModuleDto>> ListAsync(CancellationToken ct = default) =>
        (await _db.Module.AsNoTracking().OrderBy(m => m.Name).ToListAsync(ct)).Select(m => m.ToDto()).ToList();

    public async Task<ModuleDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Module.AsNoTracking().FirstOrDefaultAsync(m => m.ModuleId == id, ct);
        return e?.ToDto();
    }

    public async Task<Guid> CreateAsync(ModuleDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        var platformExists = await _db.Platform.AsNoTracking().AnyAsync(p => p.PlatformId == dto.PlatformId, ct);
        if (!platformExists) throw new InvalidOperationException("Platform not found");
        var taken = await _db.Module.AsNoTracking().AnyAsync(m => m.PlatformId == dto.PlatformId && m.Name == dto.Name, ct);
        if (taken) throw new InvalidOperationException("Module name must be unique per platform");
        var entity = dto.ToEntity();
        if (entity.ModuleId == Guid.Empty) entity.ModuleId = Guid.NewGuid();
        _db.Module.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.ModuleId;
    }

    public async Task<bool> UpdateAsync(ModuleDto dto, CancellationToken ct = default)
    {
        var id = dto.ModuleId;
        var entity = await _db.Module.FirstOrDefaultAsync(m => m.ModuleId == id, ct);
        if (entity is null) return false;
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        var taken = await _db.Module.AsNoTracking().AnyAsync(m => m.PlatformId == entity.PlatformId && m.Name == dto.Name && m.ModuleId != id, ct);
        if (taken) throw new InvalidOperationException("Module name must be unique per platform");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Status = dto.Status;
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
