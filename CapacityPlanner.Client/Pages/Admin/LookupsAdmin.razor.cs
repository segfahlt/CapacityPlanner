using System.Net.Http.Json;
using Common.CapacityPlanner.Dto;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CapacityPlanner.Client.Pages.Admin;

public partial class LookupsAdmin : ComponentBase
{
	[Inject] public required CapacityPlanner.Client.Services.LookupService Lookups { get; set; }
	[Inject] public required ISnackbar Snackbar { get; set; }
	[Inject] public required IDialogService DialogService { get; set; }

	private List<LookupTypeDto> _types = new();
	private LookupTypeDto? _selectedType;
	private LookupTypeDto? SelectedType
	{
		get => _selectedType;
		set
		{
			if (EqualityComparer<LookupTypeDto?>.Default.Equals(_selectedType, value)) return;
			_selectedType = value;
			_ = OnTypeChanged(value);
		}
	}
	private List<LookupItemDto> _items = new();

	private bool _editing;
	private bool _isCreate;
	private string _editKey = string.Empty;
	private string? _editDescription;
	private LookupItemDto? _editingOriginal;

	protected override async Task OnInitializedAsync()
	{
		_types = await Lookups.GetTypesAsync();
	}

	private Task<IEnumerable<LookupTypeDto>> SearchTypes(string value, CancellationToken _)
	=> Task.FromResult(_types.Where(t => string.IsNullOrWhiteSpace(value) || t.DisplayName.Contains(value, StringComparison.OrdinalIgnoreCase)));

	private async Task OnTypeChanged(LookupTypeDto? type)
	{
		_editing = false;
		if (type is null) { _items.Clear(); StateHasChanged(); return; }
		await LoadItemsAsync();
		StateHasChanged();
	}

	private async Task LoadItemsAsync()
	{
		if (_selectedType is null) return;
		_items = await Lookups.GetItemsAsync(_selectedType.Type);
	}

	private void StartCreate()
	{
		_isCreate = true;
		_editing = true;
		_editKey = string.Empty;
		_editDescription = null;
		_editingOriginal = null;
	}

	private void StartEdit(LookupItemDto item)
	{
		_isCreate = false;
		_editing = true;
		_editingOriginal = item;
		_editKey = item.Key;
		_editDescription = item.Description;
	}

	private async Task SaveEdit()
	{
		if (_selectedType is null) return;
		if (string.IsNullOrWhiteSpace(_editKey)) { Snackbar.Add("Key is required", Severity.Warning); return; }
		bool ok;
		if (_isCreate)
			ok = await Lookups.CreateAsync(_selectedType.Type, new LookupItemDto(_editKey, _editDescription));
		else
			ok = await Lookups.UpdateAsync(_selectedType.Type, _editingOriginal!.Key, new LookupItemDto(_editKey, _editDescription));

		if (ok)
		{
			Snackbar.Add("Saved", Severity.Success);
			_editing = false;
			await LoadItemsAsync();
		}
		else Snackbar.Add("Save failed", Severity.Error);
	}

	private void CancelEdit()
	{
		_editing = false;
	}

	private async Task ConfirmDelete(LookupItemDto item)
	{
		var confirm = await DialogService.ShowMessageBox("Confirm delete", (MarkupString)$"Delete <b>{item.Key}</b>?", yesText: "Delete", cancelText: "Cancel", options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });
		if (confirm == true)
		{
			if (await Lookups.DeleteAsync(_selectedType!.Type, item.Key))
			{
				Snackbar.Add("Deleted", Severity.Success);
				await LoadItemsAsync();
			}
			else Snackbar.Add("Delete failed", Severity.Error);
		}
	}
}
