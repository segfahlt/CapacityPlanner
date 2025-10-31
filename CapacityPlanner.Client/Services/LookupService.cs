using System.Net.Http.Json;
using Common.CapacityPlanner.Dto;

namespace CapacityPlanner.Client.Services;

public sealed class LookupService(HttpClient http)
{
 private readonly HttpClient _http = http;

 public async Task<List<LookupTypeDto>> GetTypesAsync(CancellationToken ct = default)
 => await _http.GetFromJsonAsync<List<LookupTypeDto>>("/api/lookups/types", ct) ?? new();

 public async Task<List<LookupItemDto>> GetItemsAsync(string type, CancellationToken ct = default)
 => await _http.GetFromJsonAsync<List<LookupItemDto>>($"/api/lookups/{type}", ct) ?? new();

 public async Task<bool> CreateAsync(string type, LookupItemDto item, CancellationToken ct = default)
 {
 var res = await _http.PostAsJsonAsync($"/api/lookups/{type}", item, ct);
 return res.IsSuccessStatusCode;
 }

 public async Task<bool> UpdateAsync(string type, string key, LookupItemDto item, CancellationToken ct = default)
 {
 var res = await _http.PutAsJsonAsync($"/api/lookups/{type}/{key}", item, ct);
 return res.IsSuccessStatusCode;
 }

 public async Task<bool> DeleteAsync(string type, string key, CancellationToken ct = default)
 {
 var res = await _http.DeleteAsync($"/api/lookups/{type}/{key}", ct);
 return res.IsSuccessStatusCode;
 }
}
