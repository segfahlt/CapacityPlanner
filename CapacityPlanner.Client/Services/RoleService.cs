using System.Net.Http.Json;
using Common.CapacityPlanner.Dto;

namespace CapacityPlanner.Client.Services;

public sealed class RoleService(HttpClient http)
{
 private readonly HttpClient _http = http;

 public async Task<List<RoleDto>> GetRolesAsync(CancellationToken ct = default)
 => await _http.GetFromJsonAsync<List<RoleDto>>("/api/roles", ct) ?? new();

 public async Task<bool> CreateAsync(RoleDto role, CancellationToken ct = default)
 {
 var res = await _http.PostAsJsonAsync("/api/roles", role, ct);
 return res.IsSuccessStatusCode;
 }

 public async Task<bool> UpdateAsync(Guid id, RoleDto role, CancellationToken ct = default)
 {
 var res = await _http.PutAsJsonAsync($"/api/roles/{id}", role, ct);
 return res.IsSuccessStatusCode;
 }

 public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
 {
 var res = await _http.DeleteAsync($"/api/roles/{id}", ct);
 return res.IsSuccessStatusCode;
 }
}
