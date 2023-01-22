using Editor;
using Sandbox;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NxtStudio.Collapse.NPC;

[HammerEntity]
[Title( "Zombie" )]
[Model( Model = "models/zombie/citizen_zombie_mixamo.vmdl" )]
public partial class ZombieBase : NPC, IContextActionProvider, IPersistence
{
	public float InteractionRange => 150f;
	public Color GlowColor => Color.Cyan;
	public float GlowWidth => 0.2f;

	[ResourceType( "armor" ), Property] public ArmorResource HeadArmor { get; set; }
	[ResourceType( "armor" ), Property] public ArmorResource ChestArmor { get; set; }
	[ResourceType( "armor" ), Property] public ArmorResource LegsArmor { get; set; }
	[ResourceType( "armor" ), Property] public ArmorResource FeetArmor { get; set; }

	public InventoryContainer Inventory { get; private set; }

	private ContextAction SearchAction { get; set; }

	public Vector3 EyePosition => Position + Vector3.Up * 72f;

	public ZombieBase()
	{
		SearchAction = new( "search", "Search", "textures/ui/actions/open.png" );
	}

	public string GetContextName()
	{
		return DisplayName;
	}

	public void Search( CollapsePlayer player )
	{
		UI.Storage.Open( player, GetContextName(), this, Inventory );
	}

	public IEnumerable<ContextAction> GetSecondaryActions( CollapsePlayer player )
	{
		yield break;
	}

	public ContextAction GetPrimaryAction( CollapsePlayer player )
	{
		return SearchAction;
	}
	
	public void Search( CollapsePlayer player )
	{
		UI.Storage.Open( player, GetContextName(), this, Inventory );
	}
	
	public virtual bool ShouldSaveState()
	{
		return true;
	}

	public virtual void BeforeStateLoaded()
	{

	}

	public virtual void AfterStateLoaded()
	{

	}


	public override void Spawn()
	{
		var inventory = new InventoryContainer();
		inventory.SetEntity( this );
		inventory.SetSlotLimit( (ushort)MaxItemsForSale );
		InventorySystem.Register( inventory );

		Inventory = inventory;

		AttachArmor( HeadArmor );
		AttachArmor( ChestArmor );
		AttachArmor( LegsArmor );
		AttachArmor( FeetArmor );

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		NextRestockTime = 0f;

		Tags.Add( "hover", "solid", "zombie" );

		base.Spawn();
	}

	private void AttachArmor( ArmorResource resource )
	{
		if ( !string.IsNullOrEmpty( resource.PrimaryModel ) )
		{
			var item = InventorySystem.GetDefinition( resource.UniqueId ) as ArmorItem;

			if ( item.IsValid() )
			{
				AttachArmor( resource.PrimaryModel, item );
			}
		}

		if ( !string.IsNullOrEmpty( resource.SecondaryModel ) )
		{
			var item = InventorySystem.GetDefinition( resource.UniqueId ) as ArmorItem;

			if ( item.IsValid() )
			{
				AttachArmor( resource.SecondaryModel, item );
			}
		}
	}

	private ArmorEntity AttachArmor( string modelName, ArmorItem item )
	{
		var entity = new ArmorEntity();
		entity.SetModel( modelName );
		AttachArmor( entity, item );
		return entity;
	}

	private void AttachArmor( ArmorEntity clothing, ArmorItem item )
	{
		clothing.SetParent( this, true );
		clothing.EnableShadowInFirstPerson = true;
		clothing.EnableHideInFirstPerson = true;
		clothing.Item = item;
	}
	
	private void ClientTick()
	{
		if ( IsClientOnly && TimeSinceSpawned > 120f )
		{
			Delete();
		}
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( TimeSinceSpawned > 600f )
		{
			Delete();
		}
	}
	
	private void OnItemTaken( ushort slot, InventoryItem instance )
	{
		var armor = Children.OfType<ArmorEntity>().Where( c => c.Item == instance ).ToList();

		foreach ( var entity in armor )
		{
			if ( entity.IsValid() )
			{
				entity.Delete();
			}
		}

		if ( Inventory.IsEmpty )
		{
			Delete();
		}
	}
}