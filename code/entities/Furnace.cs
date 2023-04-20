using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;

namespace NxtStudio.Collapse;

public partial class Furnace : Deployable, IContextActionProvider, ICookerEntity, IHeatEmitter
{
	public float InteractionRange => 100f;
	public bool AlwaysGlow => false;
	public Color GlowColor => Color.Orange;

	[Net] public CookingProcessor Processor { get; private set; }

	private ContextAction ExtinguishAction { get; set; }
	private ContextAction IgniteAction { get; set; }
	private ContextAction PickupAction { get; set; }
	private ContextAction OpenAction { get; set; }

	private PointLightEntity DynamicLight { get; set; }
	private Sound? ActiveSound { get; set; }

	public float EmissionRadius => 200f;
	public float HeatToEmit => Processor.IsActive ? 10f : 0f;

	public Furnace()
	{
		PickupAction = new( "pickup", "Pickup", "textures/ui/actions/pickup.png" );
		PickupAction.SetCondition( p =>
		{
			return new ContextAction.Availability
			{
				IsAvailable = Processor.IsEmpty && !Processor.IsActive
			};
		} );

		OpenAction = new( "open", "Open", "textures/ui/actions/open.png" );

		IgniteAction = new( "ignore", "Ignite", "textures/ui/actions/ignite.png" );
		ExtinguishAction = new( "extinguish", "Extinguish", "textures/ui/actions/disable.png" );
	}

	public string GetContextName()
	{
		return "Furnace";
	}

	public void Open( CollapsePlayer player )
	{
		UI.Cooking.Open( player, GetContextName(), this );
	}

	public IEnumerable<ContextAction> GetSecondaryActions( CollapsePlayer player )
	{
		yield return OpenAction;
		yield return PickupAction;
	}

	public ContextAction GetPrimaryAction( CollapsePlayer player )
	{
		if ( Processor.IsActive )
			return ExtinguishAction;
		else
			return IgniteAction;
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

				var item = InventorySystem.CreateItem<FurnaceItem>();
				player.TryGiveItem( item );
				Delete();
			}
		}
		else if ( action == IgniteAction )
		{
			if ( Game.IsServer )
			{
				if ( Processor.Fuel.IsEmpty )
				{
					UI.Thoughts.Show( To.Single( player ), "fuel_empty", "It can't be ignited without something to burn." );
					return;
				}

				Sound.FromWorld( To.Everyone, "fire.light", Position );
				Processor.Start();
			}
		}
		else if ( action == ExtinguishAction )
		{
			if ( Game.IsServer )
			{
				Sound.FromWorld( To.Everyone, "fire.extinguish", Position );
				Processor.Stop();
			}
		}
	}

	public override void Spawn()
	{
		SetModel( "models/furnace/furnace.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Processor = new();
		Processor.SetCooker( this );
		Processor.OnStarted += OnStarted;
		Processor.OnStopped += OnStopped;
		Processor.Fuel.Whitelist.Add( "fuel" );
		Processor.Input.Whitelist.Add( "ore" );

		SphereTrigger.Attach( this, EmissionRadius );

		Tags.Add( "hover", "solid" );

		base.Spawn();
	}

	public override void SerializeState( BinaryWriter writer )
	{
		base.SerializeState( writer );

		Processor.SerializeState( writer );
	}

	public override void DeserializeState( BinaryReader reader )
	{
		base.DeserializeState( reader );

		Processor.DeserializeState( reader );
	}

	public override void ClientSpawn()
	{
		Processor.SetCooker( this );
		Processor.OnStarted += OnStarted;
		Processor.OnStopped += OnStopped;

		base.ClientSpawn();
	}

	protected override void OnDestroy()
	{
		DynamicLight?.Delete();
		DynamicLight = null;

		base.OnDestroy();
	}

	[Event.Tick.Client]
	private void ClientTick()
	{
		if ( DynamicLight.IsValid() )
		{
			UpdateDynamicLight();
		}

		if ( Processor is not null )
		{
			SceneObject?.Attributes?.Set( "Brightness", Processor.IsActive ? 4f : 0f );
		}
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( Processor.IsActive )
		{
			if ( !ActiveSound.HasValue )
				ActiveSound = PlaySound( "fire.loop" );
		}
		else
		{
			ActiveSound?.Stop();
			ActiveSound = null;
		}

		Processor.Process();
	}

	private void UpdateDynamicLight()
	{
		var position = Position;
		var attachment = GetAttachment( "fire" );

		if ( attachment.HasValue )
			position = attachment.Value.Position;

		DynamicLight.Brightness = 0.1f + MathF.Sin( Time.Now * 4f ) * 0.02f;
		DynamicLight.Position = position + new Vector3( MathF.Sin( Time.Now ) * 0.5f, MathF.Cos( Time.Now ) * 0.5f, MathF.Sin( Time.Now ) * 2f );
		DynamicLight.Range = 200f + MathF.Sin( Time.Now ) * 50f;
	}

	private void OnStarted()
	{
		if ( Game.IsServer ) return;

		if ( !DynamicLight.IsValid() )
		{
			DynamicLight = new();
			DynamicLight.SetParent( this );
			DynamicLight.EnableShadowCasting = true;
			DynamicLight.DynamicShadows = true;
			DynamicLight.Color = Color.Orange;

			UpdateDynamicLight();
		}
	}

	private void OnStopped()
	{
		if ( Game.IsServer ) return;

		DynamicLight?.Delete();
		DynamicLight = null;
	}
}
