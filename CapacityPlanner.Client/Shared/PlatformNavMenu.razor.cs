using System.Net.Http.Json;
using Common.CapacityPlanner.Dto;
using Microsoft.AspNetCore.Components;

namespace CapacityPlanner.Client.Shared;

public partial class PlatformNavMenu : ComponentBase
{
 [Inject] public required HttpClient Http { get; set; }

 private bool _loading = true;
 private string? _error;
 private List<PlatformDto> _platforms = new();
 private List<ModuleDto> _modules = new();
 private readonly Dictionary<Guid, List<ModuleDto>> _platformModules = new();

 protected override async Task OnInitializedAsync()
 {
 try
 {
 _platforms = await Http.GetFromJsonAsync<List<PlatformDto>>("/api/platforms") ?? new();
 foreach (var platform in _platforms)
 _platformModules.Add(platform.PlatformId, (await Http.GetFromJsonAsync<List<ModuleDto>>($"/api/modules?platformId={platform.PlatformId}") ?? new()));

 }
 catch (Exception ex)
 {
 _error = ex.Message;
 }
 finally
 {
 _loading = false;
 }
 }
}
