using Sandbox;

namespace NxtStudio.Collapse;

[GameResource( "Component", "comp", "A type of crafting component for use with Collapse.", Icon = "build_circle" )]
[ItemClass( typeof( ComponentItem ) )]
public class ComponentResource : CollapseItemResource
{
	[Property]
	public int MaxStackSize { get; set; } = 5;

	[Property]
	public int DefaultStackSize { get; set; } = 1;
}
