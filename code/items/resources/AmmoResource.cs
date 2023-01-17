using Sandbox;

namespace NxtStudio.Collapse;

[GameResource( "Ammo", "ammo", "A type of weapon ammunition for use with Collapse.", Icon = "bento" )]
[ItemClass( typeof( AmmoItem ) )]
public class AmmoResource : LootTableResource
{
	[Property]
	public AmmoType AmmoType { get; set; } = AmmoType.None;

	[Property]
	public int MaxStackSize { get; set; } = 1;

	[Property]
	public int DefaultStackSize { get; set; } = 1;
}
