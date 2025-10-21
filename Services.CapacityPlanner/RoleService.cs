using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;

namespace Services.CapacityPlanner;

public sealed class RoleService : IRoleService
{
    private readonly CapacityPlannerContext _db;
    public RoleService(CapacityPlannerContext db) => _db = db;

    public Task<List<Role>> ListAsync(CancellationToken ct = default) =>
        _db.Role.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);

    public Task<Role?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Role.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == id, ct);

    public async Task<Guid> CreateAsync(string name, decimal? defaultUtilizationTarget, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        if (defaultUtilizationTarget is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(defaultUtilizationTarget));

        var exists = await _db.Role.AnyAsync(r => r.Name == name, ct);
        if (exists) throw new InvalidOperationException("Role name must be unique");

        var role = new Role { RoleId = Guid.NewGuid(), Name = name, DefaultUtilizationTarget = defaultUtilizationTarget };
        _db.Role.Add(role);
        await _db.SaveChangesAsync(ct);
        return role.RoleId;
    }

    public async Task<bool> UpdateAsync(Guid id, string name, decimal? defaultUtilizationTarget, CancellationToken ct = default)
    {
        var role = await _db.Role.FirstOrDefaultAsync(r => r.RoleId == id, ct);
        if (role is null) return false;
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        if (defaultUtilizationTarget is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(defaultUtilizationTarget));

        var nameTaken = await _db.Role.AnyAsync(r => r.Name == name && r.RoleId != id, ct);
        if (nameTaken) throw new InvalidOperationException("Role name must be unique");

        role.Name = name;
        role.DefaultUtilizationTarget = defaultUtilizationTarget;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _db.Role.FirstOrDefaultAsync(r => r.RoleId == id, ct);
        if (role is null) return false;
        _db.Role.Remove(role);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
