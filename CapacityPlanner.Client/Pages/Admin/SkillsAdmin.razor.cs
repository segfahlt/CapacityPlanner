using Common.CapacityPlanner.Dto;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CapacityPlanner.Client.Pages.Admin;

public partial class SkillsAdmin : ComponentBase
{
    [Inject] public required Services.SkillService Skills { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Inject] public required IDialogService DialogService { get; set; }

    private List<SkillDto> _items = new();
    private bool _loading = true;
    private string? _error;

    private bool _editing;
    private bool _isCreate;
    private Guid _editId;
    private string _editName = string.Empty;
    private string? _editDescription;
    private SkillDto? _editingOriginal;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            _loading = true;
            _error = null;
            _items = await Skills.GetSkillsAsync();
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

    private void StartCreate()
    {
        _isCreate = true;
        _editing = true;
        _editId = Guid.Empty;
        _editName = string.Empty;
        _editDescription = null;
        _editingOriginal = null;
    }

    private void StartEdit(SkillDto item)
    {
        _isCreate = false;
        _editing = true;
        _editingOriginal = item;
        _editId = item.SkillId;
        _editName = item.Name;
        _editDescription = item.Description;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_editName)) { Snackbar.Add("Name is required", Severity.Warning); return; }
        var dto = new SkillDto { SkillId = _editId, Name = _editName, Description = _editDescription };
        bool ok;
        if (_isCreate)
            ok = await Skills.CreateAsync(dto);
        else
            ok = await Skills.UpdateAsync(_editId, dto);

        if (ok)
        {
            Snackbar.Add("Saved", Severity.Success);
            _editing = false;
            await LoadAsync();
        }
        else Snackbar.Add("Save failed", Severity.Error);
    }

    private void Cancel()
    {
        _editing = false;
    }

    private async Task DeleteAsync(SkillDto item)
    {
        var confirm = await DialogService.ShowMessageBox("Confirm delete", (MarkupString)$"Delete <b>{item.Name}</b>?", yesText: "Delete", cancelText: "Cancel");
        if (confirm == true)
        {
            if (await Skills.DeleteAsync(item.SkillId))
            {
                Snackbar.Add("Deleted", Severity.Success);
                await LoadAsync();
            }
            else Snackbar.Add("Delete failed", Severity.Error);
        }
    }
}
