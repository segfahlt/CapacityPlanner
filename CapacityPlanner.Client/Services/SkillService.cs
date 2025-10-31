using System.Net.Http.Json;

using Common.CapacityPlanner.Dto;

namespace CapacityPlanner.Client.Services;

public sealed class SkillService(HttpClient http)
{
	private readonly HttpClient _http = http;

	public async Task<List<SkillDto>> GetSkillsAsync(CancellationToken ct = default)
	=> await _http.GetFromJsonAsync<List<SkillDto>>("/api/skills", ct) ?? new();

	public async Task<bool> CreateAsync(SkillDto skill, CancellationToken ct = default)
	{
		var res = await _http.PostAsJsonAsync("/api/skills", skill, ct);
		return res.IsSuccessStatusCode;
	}

	public async Task<bool> UpdateAsync(Guid id, SkillDto skill, CancellationToken ct = default)
	{
		var res = await _http.PutAsJsonAsync($"/api/skills/{id}", skill, ct);
		return res.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
	{
		var res = await _http.DeleteAsync($"/api/skills/{id}", ct);
		return res.IsSuccessStatusCode;
	}
}
