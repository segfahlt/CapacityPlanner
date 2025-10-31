using System.Net.Http.Json;

namespace CapacityPlanner.Client.Services;

public sealed class AdminService(HttpClient http)
{
 private readonly HttpClient _http = http;

 // Summary counts for dashboard
 public async Task<AdminSummary?> GetSummaryAsync(CancellationToken ct = default)
 => await _http.GetFromJsonAsync<AdminSummary>("/api/admin/summary", ct);
}

public sealed record AdminSummary(
 int Platforms,
 int Modules,
 int People,
 int Roles,
 int Skills,
 int Implementations);
