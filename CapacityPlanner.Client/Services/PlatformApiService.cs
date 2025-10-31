using System.Net.Http.Json;
using Common.CapacityPlanner.Dto;

namespace CapacityPlanner.Client.Services;

public sealed class PlatformApiService(HttpClient http)
{
 private readonly HttpClient _http = http;

 public async Task<List<PlatformDto>> GetPlatformsAsync(CancellationToken ct = default)
 => await _http.GetFromJsonAsync<List<PlatformDto>>("/api/platforms", ct) ?? new();

 public async Task<bool> CreatePlatformAsync(PlatformDto dto, CancellationToken ct = default)
 {
 var res = await _http.PostAsJsonAsync("/api/platforms", dto, ct);
 return res.IsSuccessStatusCode;
 }
 public async Task<bool> UpdatePlatformAsync(Guid id, PlatformDto dto, CancellationToken ct = default)
 {
 var res = await _http.PutAsJsonAsync($"/api/platforms/{id}", dto, ct);
 return res.IsSuccessStatusCode;
 }
 public async Task<bool> DeletePlatformAsync(Guid id, CancellationToken ct = default)
 {
 var res = await _http.DeleteAsync($"/api/platforms/{id}", ct);
 return res.IsSuccessStatusCode;
 }

 public async Task<List<ModuleDto>> GetModulesForPlatformAsync(Guid platformId, CancellationToken ct = default)
 => await _http.GetFromJsonAsync<List<ModuleDto>>($"/api/modules?platformId={platformId}", ct) ?? new();

 public async Task<bool> CreateModuleAsync(ModuleDto dto, CancellationToken ct = default)
 {
 var res = await _http.PostAsJsonAsync("/api/modules", dto, ct);
 return res.IsSuccessStatusCode;
 }
 public async Task<bool> UpdateModuleAsync(Guid id, ModuleDto dto, CancellationToken ct = default)
 {
 var res = await _http.PutAsJsonAsync($"/api/modules/{id}", dto, ct);
 return res.IsSuccessStatusCode;
 }
 public async Task<bool> DeleteModuleAsync(Guid id, CancellationToken ct = default)
 {
 var res = await _http.DeleteAsync($"/api/modules/{id}", ct);
 return res.IsSuccessStatusCode;
 }
}
