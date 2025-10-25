using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class ImplementationService : IImplementationService
{
 private readonly CapacityPlannerContext _db;
 public ImplementationService(CapacityPlannerContext db) => _db = db;

 public async Task<List<ImplementationDto>> ListAsync(Guid? moduleId = null, Guid? platformId = null, CancellationToken ct = default)
 {
 var q = _db.Implementation.AsNoTracking().AsQueryable();
 if (moduleId is Guid m) q = q.Where(x => x.ModuleId == m);
 if (platformId is Guid p) q = q.Where(x => x.PlatformId == p);
 var list = await q.OrderBy(x => x.ClientName).ToListAsync(ct);
 return list.Select(x => x.ToDto()).ToList();
 }

 public async Task<ImplementationDto?> GetAsync(Guid id, CancellationToken ct = default)
 {
 var e = await _db.Implementation.AsNoTracking().FirstOrDefaultAsync(x => x.ImplementationId == id, ct);
 return e?.ToDto();
 }

 public async Task<Guid> CreateAsync(ImplementationDto dto, CancellationToken ct = default)
 {
 if (string.IsNullOrWhiteSpace(dto.ClientName)) throw new ArgumentException("ClientName required", nameof(dto.ClientName));
 var exists = await _db.Implementation.AnyAsync(i => i.PlatformId == dto.PlatformId && i.ClientName == dto.ClientName, ct);
 if (exists) throw new InvalidOperationException("Implementation client must be unique per platform");
 var entity = dto.ToEntity();
 if (entity.ImplementationId == Guid.Empty) entity.ImplementationId = Guid.NewGuid();
 _db.Implementation.Add(entity);
 await _db.SaveChangesAsync(ct);
 return entity.ImplementationId;
 }

 public async Task<bool> UpdateAsync(ImplementationDto dto, CancellationToken ct = default)
 {
 var id = dto.ImplementationId;
 var entity = await _db.Implementation.FirstOrDefaultAsync(i => i.ImplementationId == id, ct);
 if (entity is null) return false;
 if (string.IsNullOrWhiteSpace(dto.ClientName)) throw new ArgumentException("ClientName required", nameof(dto.ClientName));
 var taken = await _db.Implementation.AnyAsync(i => i.PlatformId == entity.PlatformId && i.ClientName == dto.ClientName && i.ImplementationId != id, ct);
 if (taken) throw new InvalidOperationException("Implementation client must be unique per platform");
 entity.ClientName = dto.ClientName;
 entity.Description = dto.Description;
 entity.ModuleId = dto.ModuleId;
 entity.StartDate = dto.StartDate;
 entity.GoLiveDate = dto.GoLiveDate;
 entity.Status = dto.Status;
 await _db.SaveChangesAsync(ct);
 return true;
 }

 public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
 {
 var entity = await _db.Implementation.FirstOrDefaultAsync(i => i.ImplementationId == id, ct);
 if (entity is null) return false;
 _db.Implementation.Remove(entity);
 await _db.SaveChangesAsync(ct);
 return true;
 }
}
