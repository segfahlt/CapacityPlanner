using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;

namespace Services.CapacityPlanner;

public sealed class PlatformService : IPlatformService
{
    private readonly CapacityPlannerContext _db;
    public PlatformService(CapacityPlannerContext db) => _db = db;

    public Task<List<Platform>> ListAsync(CancellationToken ct = default) =>
        _db.Platform.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);

    public Task<Platform?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Platform.AsNoTracking().FirstOrDefaultAsync(p => p.PlatformId == id, ct);

    public async Task<Guid> CreateAsync(string name, string? description, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        var exists = await _db.Platform.AnyAsync(p => p.Name == name, ct);
        if (exists) throw new InvalidOperationException("Platform name must be unique");
        var entity = new Platform { PlatformId = Guid.NewGuid(), Name = name, Description = description };
        _db.Platform.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PlatformId;
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string? description, CancellationToken ct = default)
    {
        var entity = await _db.Platform.FirstOrDefaultAsync(p => p.PlatformId == id, ct);
        if (entity is null) return false;
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        var taken = await _db.Platform.AnyAsync(p => p.Name == name && p.PlatformId != id, ct);
        if (taken) throw new InvalidOperationException("Platform name must be unique");
        entity.Name = name;
        entity.Description = description;
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
