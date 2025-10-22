using Persist.CapacityPlanner.DbModel.Entities;

namespace Services.CapacityPlanner.Abstraction;

public interface IImplementationTemplateService
{
    // Basic create
    Task<Guid> CreateTemplateAsync(string name, string? description, string? category, bool isActive, CancellationToken ct = default);
    Task<Guid> AddPhaseAsync(Guid templateId, string phaseName, int sequence, int? defaultDurationWeeks, int? defaultStartOffsetWeeks, string? defaultStatus, string? notes, CancellationToken ct = default);
    Task<Guid> AddPhaseDemandAsync(Guid phaseId, Guid roleId, decimal? defaultFte, string? notes, CancellationToken ct = default);
    Task<Guid> AddPhaseDemandSkillAsync(Guid phaseDemandId, Guid skillId, string? priorityLevel, string? notes, CancellationToken ct = default);

    // Read helpers
    Task<ImplementationTemplate?> GetTemplateAsync(Guid templateId, bool includeGraph = false, CancellationToken ct = default);

    // Composite create from request
    Task<Guid> CreateTemplateAsync(Contracts.ImplementationTemplateCreateRequest request, CancellationToken ct = default);
}
