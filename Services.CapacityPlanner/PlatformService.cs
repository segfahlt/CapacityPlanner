using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class PlatformService : IPlatformService
{
    private readonly CapacityPlannerContext _db;
    public PlatformService(CapacityPlannerContext db) => _db = db;

    public async Task<List<PlatformDto>> ListAsync(CancellationToken ct = default) =>
        (await _db.Platform.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct)).Select(p => p.ToDto()).ToList();

    public async Task<PlatformDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Platform.AsNoTracking().FirstOrDefaultAsync(p => p.PlatformId == id, ct);
        return e?.ToDto();
    }

    public async Task<Guid> CreateAsync(PlatformDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        var exists = await _db.Platform.AnyAsync(p => p.Name == dto.Name, ct);
        if (exists) throw new InvalidOperationException("Platform name must be unique");
        var entity = dto.ToEntity();
        if (entity.PlatformId == Guid.Empty) entity.PlatformId = Guid.NewGuid();
        _db.Platform.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PlatformId;
    }

    public async Task<bool> UpdateAsync(PlatformDto dto, CancellationToken ct = default)
    {
        var id = dto.PlatformId;
        var entity = await _db.Platform.FirstOrDefaultAsync(p => p.PlatformId == id, ct);
        if (entity is null) return false;
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        var taken = await _db.Platform.AnyAsync(p => p.Name == dto.Name && p.PlatformId != id, ct);
        if (taken) throw new InvalidOperationException("Platform name must be unique");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Platform.FirstOrDefaultAsync(p => p.PlatformId == id, ct);
        if (entity is null) return false;
        _db.Platform.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
