﻿using Sandbox;
using System.Collections.Generic;
using System.IO;

namespace NxtStudio.Collapse;

public partial class StorageCrate : Deployable, IContextActionProvider
{
	public float InteractionRange => 100f;
	public Color GlowColor => Color.Green;
	public bool AlwaysGlow => false;

	[Net] private NetInventoryContainer InternalInventory { get; set; }
	public InventoryContainer Inventory => InternalInventory.Value;

	private ContextAction PickupAction { get; set; }
	private ContextAction OpenAction { get; set; }

	[Net] public bool IsEmpty { get; private set; }

	public StorageCrate()
	{
		PickupAction = new( "pickup", "Pickup", "textures/ui/actions/pickup.png" );
		PickupAction.SetCondition( p =>
		{
			return new ContextAction.Availability
			{
				IsAvailable = IsEmpty
			};
		} );

		OpenAction = new( "open", "Open", "textures/ui/actions/open.png" );
	}

	public string GetContextName()
	{
		return "Storage Crate";
	}

	public void Open( CollapsePlayer player )
	{
		UI.Storage.Open( player, GetContextName(), this, Inventory );
	}

	public IEnumerable<ContextAction> GetSecondaryActions( CollapsePlayer player )
	{
		yield return OpenAction;
		yield return PickupAction;
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
		else if ( action == PickupAction )
		{
			if ( Game.IsServer )
			{
				Sound.FromScreen( To.Single( player ), "inventory.move" );

				var item = InventorySystem.CreateItem<StorageCrateItem>();
				player.TryGiveItem( item );
				Delete();
			}
		}
	}

	public override void SerializeState( BinaryWriter writer )
	{
		base.SerializeState( writer );

		writer.Write( Inventory );
	}

	public override void DeserializeState( BinaryReader reader )
	{
		base.DeserializeState( reader );

		var container = reader.ReadInventoryContainer();
		InternalInventory = new( container );
		IsEmpty = container.IsEmpty;
	}

	public override void Spawn()
	{
		SetModel( "models/citizen_props/crate01.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		var inventory = new InventoryContainer();
		inventory.SetEntity( this );
		inventory.SetSlotLimit( 16 );
		inventory.SlotChanged += OnSlotChanged;
		InventorySystem.Register( inventory );

		InternalInventory = new NetInventoryContainer( inventory );
		IsEmpty = inventory.IsEmpty;

		Tags.Add( "hover", "solid" );

		base.Spawn();
	}

	private void OnSlotChanged( ushort slot )
	{
		if ( Game.IsServer )
		{
			IsEmpty = Inventory.IsEmpty;
		}
	}
}
