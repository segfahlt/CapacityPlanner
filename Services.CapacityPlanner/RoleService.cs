using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class RoleService : IRoleService
{
    private readonly CapacityPlannerContext _db;
    public RoleService(CapacityPlannerContext db) => _db = db;

    public async Task<List<RoleDto>> ListAsync(CancellationToken ct = default) =>
        (await _db.Role.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct)).Select(r => r.ToDto()).ToList();

    public async Task<RoleDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Role.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == id, ct);
        return e?.ToDto();
    }

    public async Task<Guid> CreateAsync(RoleDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        if (dto.DefaultUtilizationTarget is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(dto.DefaultUtilizationTarget));

        var exists = await _db.Role.AnyAsync(r => r.Name == dto.Name, ct);
        if (exists) throw new InvalidOperationException("Role name must be unique");

        var role = new Role { RoleId = dto.RoleId == Guid.Empty ? Guid.NewGuid() : dto.RoleId, Name = dto.Name, DefaultUtilizationTarget = dto.DefaultUtilizationTarget };
        _db.Role.Add(role);
        await _db.SaveChangesAsync(ct);
        return role.RoleId;
    }

    public async Task<bool> UpdateAsync(RoleDto dto, CancellationToken ct = default)
    {
        var roleId = dto.RoleId;
        var role = await _db.Role.FirstOrDefaultAsync(r => r.RoleId == roleId, ct);
        if (role is null) return false;
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        if (dto.DefaultUtilizationTarget is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(dto.DefaultUtilizationTarget));

        var nameTaken = await _db.Role.AnyAsync(r => r.Name == dto.Name && r.RoleId != roleId, ct);
        if (nameTaken) throw new InvalidOperationException("Role name must be unique");

        role.Name = dto.Name;
        role.DefaultUtilizationTarget = dto.DefaultUtilizationTarget;
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
