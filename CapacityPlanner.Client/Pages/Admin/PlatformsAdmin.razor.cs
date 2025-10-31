using Common.CapacityPlanner.Dto;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CapacityPlanner.Client.Pages.Admin;

public partial class PlatformsAdmin : ComponentBase
{
    [Inject] public required Services.PlatformApiService Api { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Inject] public required IDialogService Dialogs { get; set; }

    private List<PlatformDto> _platforms = new();
    private List<ModuleDto> _modules = new();
    private PlatformDto? _selectedPlatform;

    private bool _platformEditing;
    private bool _platformIsCreate;
    private Guid _platformEditId;
    private string _platformEditName = string.Empty;
    private string? _platformEditDescription;

    private bool _moduleEditing;
    private bool _moduleIsCreate;
    private Guid _moduleEditId;
    private string _moduleEditName = string.Empty;
    private string? _moduleEditDescription;
    private string? _moduleEditStatus;

    protected override async Task OnInitializedAsync()
    {
        await LoadPlatformsAsync();
    }

    private async Task LoadPlatformsAsync()
    {
        _platforms = await Api.GetPlatformsAsync();
        if (_selectedPlatform is not null)
        {
            _selectedPlatform = _platforms.FirstOrDefault(p => p.PlatformId == _selectedPlatform.PlatformId);
            if (_selectedPlatform is not null)
                _modules = await Api.GetModulesForPlatformAsync(_selectedPlatform.PlatformId);
            else
                _modules.Clear();
        }
    }

    private async Task OnPlatformRowClick(TableRowClickEventArgs<PlatformDto> args)
    {
        _selectedPlatform = args.Item;
        _moduleEditing = false;
        _modules = await Api.GetModulesForPlatformAsync(_selectedPlatform.PlatformId);
    }

    private void StartCreatePlatform()
    {
        _platformIsCreate = true;
        _platformEditing = true;
        _platformEditId = Guid.Empty;
        _platformEditName = string.Empty;
        _platformEditDescription = null;
    }
    private void StartEditPlatform(PlatformDto p)
    {
        _platformIsCreate = false;
        _platformEditing = true;
        _platformEditId = p.PlatformId;
        _platformEditName = p.Name;
        _platformEditDescription = p.Description;
    }
    private async Task SavePlatformAsync()
    {
        if (string.IsNullOrWhiteSpace(_platformEditName)) { Snackbar.Add("Name is required", Severity.Warning); return; }
        var dto = new PlatformDto { PlatformId = _platformEditId, Name = _platformEditName, Description = _platformEditDescription };
        bool ok = _platformIsCreate
            ? await Api.CreatePlatformAsync(dto)
            : await Api.UpdatePlatformAsync(_platformEditId, dto);
        if (ok)
        {
            Snackbar.Add("Platform saved", Severity.Success);
            _platformEditing = false;
            await LoadPlatformsAsync();
        }
        else Snackbar.Add("Save failed", Severity.Error);
    }
    private void CancelPlatform() => _platformEditing = false;
    private async Task DeletePlatformAsync(PlatformDto p)
    {
        var confirm = await Dialogs.ShowMessageBox("Confirm delete", (MarkupString)$"Delete <b>{p.Name}</b>?", yesText: "Delete", cancelText: "Cancel");
        if (confirm == true)
        {
            if (await Api.DeletePlatformAsync(p.PlatformId))
            {
                Snackbar.Add("Deleted", Severity.Success);
                if (_selectedPlatform?.PlatformId == p.PlatformId) { _selectedPlatform = null; _modules.Clear(); }
                await LoadPlatformsAsync();
            }
            else Snackbar.Add("Delete failed", Severity.Error);
        }
    }

    private void StartCreateModule()
    {
        if (_selectedPlatform is null) return;
        _moduleIsCreate = true;
        _moduleEditing = true;
        _moduleEditId = Guid.Empty;
        _moduleEditName = string.Empty;
        _moduleEditDescription = null;
        _moduleEditStatus = null;
    }
    private void StartEditModule(ModuleDto m)
    {
        _moduleIsCreate = false;
        _moduleEditing = true;
        _moduleEditId = m.ModuleId;
        _moduleEditName = m.Name;
        _moduleEditDescription = m.Description;
        _moduleEditStatus = m.Status;
    }
    private async Task SaveModuleAsync()
    {
        if (_selectedPlatform is null) return;
        if (string.IsNullOrWhiteSpace(_moduleEditName)) { Snackbar.Add("Name is required", Severity.Warning); return; }
        var dto = new ModuleDto { ModuleId = _moduleEditId, PlatformId = _selectedPlatform.PlatformId, Name = _moduleEditName, Description = _moduleEditDescription, Status = _moduleEditStatus };
        bool ok = _moduleIsCreate
            ? await Api.CreateModuleAsync(dto)
            : await Api.UpdateModuleAsync(_moduleEditId, dto);
        if (ok)
        {
            Snackbar.Add("Module saved", Severity.Success);
            _moduleEditing = false;
            _modules = await Api.GetModulesForPlatformAsync(_selectedPlatform.PlatformId);
        }
        else Snackbar.Add("Save failed", Severity.Error);
    }
    private void CancelModule() => _moduleEditing = false;
    private async Task DeleteModuleAsync(ModuleDto m)
    {
        var confirm = await Dialogs.ShowMessageBox("Confirm delete", (MarkupString)$"Delete <b>{m.Name}</b>?", yesText: "Delete", cancelText: "Cancel");
        if (confirm == true)
        {
            if (await Api.DeleteModuleAsync(m.ModuleId))
            {
                Snackbar.Add("Deleted", Severity.Success);
                _modules = await Api.GetModulesForPlatformAsync(_selectedPlatform!.PlatformId);
            }
            else Snackbar.Add("Delete failed", Severity.Error);
        }
    }
}
