using Sandbox;
using System.Collections.Generic;

namespace NxtStudio.Collapse;

public class CollapseItemResource : ItemResource
{
	/// <summary>
	/// Can this item be recycled?
	/// </summary>
	[Property]
	public bool IsRecyclable { get; set; }

	/// <summary>
	/// The unique item ids and their quantities to output as a result of recycling this item.
	/// </summary>
	[Property, ShowIf( nameof( IsRecyclable ), true )] public Dictionary<string, int> RecycleOutput { get; set; }

	/// <summary>
	/// Can this item be purchased from a trader?
	/// </summary>
	[Property]
	public bool IsPurchasable { get; set; }

	/// <summary>
	/// How much Salvage does it cost to purchase this item from a trader?
	/// </summary>
	[Property, ShowIf( nameof( IsPurchasable ), true )]
	public int SalvageCost { get; set; } = 0;

	/// <summary>
	/// How many of this item should be stacked when stocked by a trader?
	/// </summary>
	[Property, ShowIf( nameof( IsPurchasable ), true )]
	public RangedFloat StockStackSize { get; set; } = 1f;

	/// <summary>
	/// What is the chance that this item will be stocked by a trader?
	/// </summary>
	[Property, Range( 0f, 1f ), ShowIf( nameof( IsPurchasable ), true )]
	public float StockChance { get; set; } = 0.5f;

	/// <summary>
	/// Is this item lootable from a loot spawner?
	/// </summary>
	[Property]
	public bool IsLootable { get; set; }

	/// <summary>
	/// What is the chance that this item will be spawned as loot?
	/// </summary>
	[Property, Range( 0f, 1f ), ShowIf( nameof( IsLootable ), true )]
	public float LootChance { get; set; } = 0.5f;

	/// <summary>
	/// How many of this item should be stacked when spawned as loot?
	/// </summary>
	[Property, ShowIf( nameof( IsLootable ), true )]
	public RangedFloat LootStackSize { get; set; } = 1f;
}