using System.Net.Http.Json;

using Common.CapacityPlanner.Dto;

using Microsoft.AspNetCore.Components;

namespace CapacityPlanner.Client.Shared;

public partial class TreeNav : ComponentBase
{
	[Inject] public required HttpClient Http { get; set; }
	[Inject] public required NavigationManager Nav { get; set; }

	private List<MudBlazor.TreeItemData<TreeNode>> _roots = new();

	private TreeNode? _selectedBacking;
	private TreeNode? _selected
	{
		get => _selectedBacking;
		set
		{
			_selectedBacking = value;
			if (value?.Type == NodeType.Implementation)
			{
				Nav.NavigateTo($"/implementations/{value.Id}");
			}
		}
	}

	protected override async Task OnInitializedAsync()
	{
		var platforms = await Http.GetFromJsonAsync<List<PlatformDto>>("/api/platforms");
		_roots = platforms?.Select(p => new MudBlazor.TreeItemData<TreeNode>
		{
			Value = new TreeNode(p.PlatformId, p.Name, NodeType.Platform, true)
		}).ToList() ?? new();
	}

	private async Task<IEnumerable<MudBlazor.TreeItemData<TreeNode>>> LoadChildrenAsync(MudBlazor.TreeItemData<TreeNode> parent)
	{
		switch (parent.Value.Type)
		{
			case NodeType.Platform:
				var mods = await Http.GetFromJsonAsync<List<ModuleDto>>($"/api/modules?platformId={parent.Value.Id}");
				return mods?.Select(m => new MudBlazor.TreeItemData<TreeNode>
				{
					Value = new TreeNode(m.ModuleId, m.Name, NodeType.Module, true)
				}) ?? Enumerable.Empty<MudBlazor.TreeItemData<TreeNode>>();

			case NodeType.Module:
				var impls = await Http.GetFromJsonAsync<List<ImplementationDto>>($"/api/implementations?moduleId={parent.Value.Id}");
				return impls?.Select(i => new MudBlazor.TreeItemData<TreeNode>
				{
					Value = new TreeNode(i.ImplementationId, i.ClientName, NodeType.Implementation, false)
				}) ?? Enumerable.Empty<MudBlazor.TreeItemData<TreeNode>>();
		}
		return Enumerable.Empty<MudBlazor.TreeItemData<TreeNode>>();
	}

	private static string GetIcon(NodeType t) => t switch
	{
		NodeType.Platform => MudBlazor.Icons.Material.Filled.Layers,
		NodeType.Module => MudBlazor.Icons.Material.Filled.ViewModule,
		NodeType.Implementation => MudBlazor.Icons.Material.Filled.BuildCircle,
		_ => MudBlazor.Icons.Material.Filled.Folder
	};

	public record TreeNode(Guid Id, string Text, NodeType Type, bool HasChildren);
	public enum NodeType { Platform, Module, Implementation }
}
