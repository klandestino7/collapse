﻿using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace NxtStudio.Collapse;

public abstract partial class LootSpawner : ModelEntity, IContextActionProvider, IPersistence
{
	public float InteractionRange => 150f;
	public Color GlowColor => Color.Green;
	public float GlowWidth => 0.2f;

	[Net] public TimeUntil NextRestockTime { get; private set; }

	public InventoryContainer Inventory { get; private set; }

	public virtual string Title { get; set; } = "Loot Spawner";
	public virtual float RestockTime { get; set; } = 30f;
	public virtual int SlotLimit { get; set; } = 6;
	public virtual float MinStockChance { get; set; } = 0f;
	public virtual float MaxStockChance { get; set; } = 1f;

	private ContextAction OpenAction { get; set; }
	private bool IsHidden { get; set; }

	public LootSpawner()
	{
		OpenAction = new( "open", "Open", "textures/ui/actions/open.png" );
	}

	public string GetContextName()
	{
		return Title;
	}

	public void Open( CollapsePlayer player )
	{
		UI.Storage.Open( player, GetContextName(), this, Inventory );
	}

	public IEnumerable<ContextAction> GetSecondaryActions( CollapsePlayer player )
	{
		yield break;
	}

	public ContextAction GetPrimaryAction( CollapsePlayer player )
	{
		return OpenAction;
	}

	public virtual void OnContextAction( CollapsePlayer player, ContextAction action )
	{
		if ( action == OpenAction )
		{
			if ( Game.IsServer )
			{
				Open( player );
			}
		}
	}

	public virtual bool ShouldSaveState()
	{
		return true;
	}

	public virtual void SerializeState( BinaryWriter writer )
	{
		writer.Write( IsHidden );
		writer.Write( NextRestockTime.Fraction );
		writer.Write( Inventory );
	}

	public virtual void DeserializeState( BinaryReader reader )
	{
		IsHidden = reader.ReadBoolean();
		NextRestockTime = RestockTime * reader.ReadSingle();

		Inventory = reader.ReadInventoryContainer();
		Inventory.IsTakeOnly = true;
		Inventory.SlotChanged += OnSlotChanged;
		Inventory.SetSlotLimit( (ushort)SlotLimit );
	}

	public virtual void BeforeStateLoaded()
	{

	}

	public virtual void AfterStateLoaded()
	{
		if ( IsHidden )
			Hide();
		else
			Show();
	}

	public override void OnNewModel( Model model )
	{
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		base.OnNewModel( model );
	}

	public override void Spawn()
	{
		var inventory = new InventoryContainer();
		inventory.IsTakeOnly = true;
		inventory.SetEntity( this );
		inventory.SetSlotLimit( (ushort)SlotLimit );
		inventory.SlotChanged += OnSlotChanged;
		InventorySystem.Register( inventory );

		Inventory = inventory;

		NextRestockTime = 0f;
		Hide();

		Tags.Add( "hover", "solid" );

		base.Spawn();
	}

	protected virtual void Restock()
	{
		var possibleItems = InventorySystem.GetDefinitions()
			.OfType<ILootSpawnerItem>()
			.Where( i => i.IsLootable )
			.Where( i => i.StockChance > 0f && i.StockChance > MinStockChance && i.StockChance < MaxStockChance );

		if ( !possibleItems.Any() ) return;

		var itemsToSpawn = Game.Random.Int( 1, SlotLimit );

		for ( var i = 0; i < itemsToSpawn; i++ )
		{
			var u = possibleItems.Sum( p => p.StockChance );
			var r = Game.Random.Float() * u;
			var s = 0f;

			foreach ( var item in possibleItems )
			{
				s += item.StockChance;

				if ( r < s )
				{
					var instance = InventorySystem.CreateItem( item.UniqueId );
					instance.StackSize = (ushort)item.AmountToStock;
					Inventory.Stack( instance );
					break;
				}
			}
		}
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( NextRestockTime && IsHidden )
		{
			if ( !IsAreaClear() )
			{
				NextRestockTime = RestockTime;
				return;
			}

			Restock();
			Show();
		}
	}

	private void OnSlotChanged( ushort slot )
	{
		if ( IsValid && Inventory.IsEmpty )
		{
			NextRestockTime = RestockTime;
			Hide();
		}
	}

	private bool IsAreaClear()
	{
		var entities = FindInSphere( Position, 32f ).Where( e => !e.Equals( this ) );
		return !entities.Any();
	}

	private void Hide()
	{
		EnableAllCollisions = false;
		EnableDrawing = false;
		IsHidden = true;
	}

	private void Show()
	{
		EnableAllCollisions = true;
		EnableDrawing = true;
		IsHidden = false;
	}
}
