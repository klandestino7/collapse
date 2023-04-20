using Sandbox;
using Sandbox.Component;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NxtStudio.Collapse;

public abstract partial class LootSpawner : ModelEntity, IContextActionProvider, IPersistence
{
	[ConCmd.Server( "fsk.loot.restock" )]
	public static void RestockAll()
	{
		foreach ( var s in All.OfType<LootSpawner>() )
		{
			if ( s.IsHidden )
			{
				s.Restock();
				s.Show();
			}
		}

		foreach ( var s in All.OfType<Trader>() )
		{
			s.Restock();
		}
	}

	public float InteractionRange => 100f;
	public Color GlowColor => Color.Green;
	public bool AlwaysGlow => true;

	[Net] public TimeUntil NextRestockTime { get; private set; }

	public InventoryContainer Inventory { get; private set; }

	public virtual string OpeningSound { get; set; } = "rummage.loot";
	public virtual string BreakSound { get; set; } = "fsk.break.wood";
	public virtual string Title { get; set; } = "Loot Spawner";
	public virtual float RestockTime { get; set; } = 30f;
	public virtual int SlotLimit { get; set; } = 6;
	public virtual float MinLootChance { get; set; } = 0f;
	public virtual float MaxLootChance { get; set; } = 1f;

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
				var timedAction = new TimedActionInfo( Open );

				timedAction.SoundName = OpeningSound;
				timedAction.Title = "Opening";
				timedAction.Origin = Position;
				timedAction.Duration = 1f;
				timedAction.Icon = "textures/ui/actions/open.png";

				player.StartTimedAction( timedAction );
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

		Inventory = reader.ReadInventoryContainer( Inventory );
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

		if ( Game.IsServer )
		{
			UpdateNavBlocker();
		}

		base.OnNewModel( model );
	}

	public override void Spawn()
	{
		var inventory = new InventoryContainer();
		inventory.IsTakeOnly = true;
		inventory.SetEntity( this );
		inventory.SetSlotLimit( (ushort)SlotLimit );
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
			.Where( i => i.LootChance > 0f && i.LootChance >= MinLootChance && i.LootChance <= MaxLootChance );

		if ( !possibleItems.Any() ) return;

		var itemsToSpawn = Game.Random.Int( SlotLimit / 2, SlotLimit );
		var spawnedItems = new HashSet<ILootSpawnerItem>();

		for ( var i = 0; i < itemsToSpawn; i++ )
		{
			var unspawnedItems = possibleItems.Except( spawnedItems );
			var u = unspawnedItems.Sum( p => p.LootChance );
			var r = Game.Random.Float() * u;
			var s = 0f;

			foreach ( var item in unspawnedItems )
			{
				s += item.LootChance;

				if ( r < s )
				{
					var instance = InventorySystem.CreateItem( item.UniqueId );
					instance.StackSize = (ushort)item.LootStackSize;
					Inventory.Stack( instance );
					spawnedItems.Add( item );
					break;
				}
			}
		}
	}

	protected void UpdateNavBlocker()
	{
		Game.AssertServer();
		Components.RemoveAny<NavBlocker>();
		Components.Add( new NavBlocker() );
		Event.Run( "fsk.navblocker.added", Position );
	}

	protected void RemoveNavBlocker()
	{
		Game.AssertServer();
		Components.RemoveAny<NavBlocker>();
		Event.Run( "fsk.navblocker.removed", Position );
	}

	protected override void OnDestroy()
	{
		if ( Game.IsServer )
		{
			RemoveNavBlocker();
		}

		base.OnDestroy();
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

		if ( !IsHidden && Inventory.IsEmpty )
		{
			if ( !string.IsNullOrEmpty( BreakSound ) )
			{
				PlaySound( BreakSound );
			}

			Breakables.Break( this );
			NextRestockTime = RestockTime;
			Hide();
		}
	}

	private bool IsAreaClear()
	{
		var entities = FindInSphere( Position, 32f ).Where( e => !e.IsFromMap && !e.Equals( this ) );
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
