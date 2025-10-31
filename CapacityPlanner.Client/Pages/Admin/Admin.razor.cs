using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CapacityPlanner.Client.Pages.Admin;

public partial class Admin : ComponentBase
{
 [Inject] public required HttpClient Http { get; set; }
 [Inject] public required ISnackbar Snackbar { get; set; }

 protected override async Task OnInitializedAsync()
 {
 try
 {
 _ = await Http.GetFromJsonAsync<object>("/api/admin");
 }
 catch (Exception ex)
 {
 Snackbar.Add($"Admin API not available: {ex.Message}", Severity.Error);
 }
 }
}
