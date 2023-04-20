using Editor;
using Sandbox;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NxtStudio.Collapse;

[HammerEntity]
[Title( "Zombie" )]
[Model( Model = "models/zombie/charger/charger_zombie.vmdl" )]
public partial class ZombieBase : NPC, IPersistence
{
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

	// public string GetContextName()
	// {
	// 	return DisplayName;
	// }

	// public void Search( ZombieBase zombie )
	// {
	// 	UI.Storage.Open( zombie, GetContextName(), this, Inventory );
	// }

	// public IEnumerable<ContextAction> GetSecondaryActions( ZombieBase zombie )
	// {
	// 	yield break;
	// }

	// public ContextAction GetPrimaryAction( ZombieBase zombie )
	// {
	// 	return SearchAction;
	// }

	// public virtual void OnContextAction( ZombieBase zombie, ContextAction action )
	// {
	// 	if ( action == SearchAction )
	// 	{
	// 		if ( Game.IsServer )
	// 		{
	// 			Search( zombie );
	// 		}
	// 	}
	// }
	
	// public void Search( ZombieBase zombie )
	// {
	// 	UI.Storage.Open( zombie, GetContextName(), this, Inventory );
	// }
	
	public virtual bool ShouldSaveState()
	{
		return true;
	}

	public virtual void SerializeState( BinaryWriter writer )
	{
		writer.Write( Inventory );
	}

	public virtual void DeserializeState( BinaryReader reader )
	{
		Inventory = reader.ReadInventoryContainer();
		Inventory.SetSlotLimit( 15 );
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
		inventory.SetSlotLimit( 15 );
		InventorySystem.Register( inventory );

		Inventory = inventory;

		AttachArmor( HeadArmor );
		AttachArmor( ChestArmor );
		AttachArmor( LegsArmor );
		AttachArmor( FeetArmor );

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		EnableSolidCollisions = false;

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
		// if ( IsClientOnly && TimeSinceSpawned > 120f )
		// {
		// 	Delete();
		// }
	}

	protected override void ServerTick()
	{
		base.ServerTick();


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