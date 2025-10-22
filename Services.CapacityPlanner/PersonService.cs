using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class PersonService : IPersonService
{
    private readonly CapacityPlannerContext _db;
    public PersonService(CapacityPlannerContext db) => _db = db;

    public async Task<List<PersonDto>> ListAsync(CancellationToken ct = default) =>
        (await _db.Person.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct)).Select(p => p.ToDto()).ToList();

    public async Task<PersonDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Person.AsNoTracking().FirstOrDefaultAsync(p => p.PersonId == id, ct);
        return e?.ToDto();
    }

    public async Task<Guid> CreateAsync(PersonDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        // Optional FK to Lookup_EmploymentType by string key – validate exists if provided
        if (!string.IsNullOrWhiteSpace(dto.EmploymentType))
        {
            var exists = await _db.LookupEmploymentType.AsNoTracking().AnyAsync(e => e.EmploymentType == dto.EmploymentType, ct);
            if (!exists) throw new InvalidOperationException("EmploymentType not found");
        }
        var entity = new Person
        {
            PersonId = dto.PersonId == Guid.Empty ? Guid.NewGuid() : dto.PersonId,
            Name = dto.Name,
            EmploymentType = dto.EmploymentType,
            IsActive = dto.IsActive,
            Location = dto.Location
        };
        _db.Person.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PersonId;
    }

    public async Task<bool> UpdateAsync(PersonDto dto, CancellationToken ct = default)
    {
        var id = dto.PersonId;
        var entity = await _db.Person.FirstOrDefaultAsync(p => p.PersonId == id, ct);
        if (entity is null) return false;
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        if (!string.IsNullOrWhiteSpace(dto.EmploymentType))
        {
            var exists = await _db.LookupEmploymentType.AsNoTracking().AnyAsync(e => e.EmploymentType == dto.EmploymentType, ct);
            if (!exists) throw new InvalidOperationException("EmploymentType not found");
        }
        entity.Name = dto.Name;
        entity.EmploymentType = dto.EmploymentType;
        entity.IsActive = dto.IsActive;
        entity.Location = dto.Location;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Person.FirstOrDefaultAsync(p => p.PersonId == id, ct);
        if (entity is null) return false;
        _db.Person.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
