// sandbox
global using Sandbox;
global using Sandbox.Component;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
// system
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.ComponentModel;
global using System.Linq;
global using System.Threading.Tasks;


namespace NxtStudio.Collapse;

public partial class CollapseGame : GameManager
{
	public static CollapseGame Entity => Current as CollapseGame;

	[ConVar.Server( "fsk.autosave", Saved = true )]
	public static bool ShouldAutoSave { get; set; } = true;
	
	private TimeUntil NextDespawnItems { get; set; }
	private TimeUntil NextAutoSave { get; set; }
	private TopDownCamera Camera { get; set; }
	private bool HasLoadedWorld { get; set; }

	public CollapseGame() : base()
	{
		
	} 
	
	public override void LoadSavedGame( SavedGame save )
	{
		Log.Info( "[Collapse] Loading world..." );

		using var s = new MemoryStream( save.Data );
		using var r = new BinaryReader( s );

		PersistenceSystem.LoadAll( r );
	}

	public override void Spawn()
	{
		InventorySystem.Initialize();
		base.Spawn();
	}

	public override void ClientSpawn()
	{
		InventorySystem.Initialize();

		ItemTag.Register( "deployable", "Deployable", ItemColors.Deployable );
		ItemTag.Register( "consumable", "Consumable", ItemColors.Consumable );
		ItemTag.Register( "tool", "Tool", ItemColors.Tool );

		Game.RootPanel?.Delete( true );
		Game.RootPanel = new UI.Hud();

		Camera = new();

		base.ClientSpawn();
	}

	public override void ClientJoined( IClient client )
	{
		InventorySystem.ClientJoined( client );

		var pawn = All.OfType<CollapsePlayer>()
			.Where( p => p.SteamId == client.SteamId )
			.FirstOrDefault();

		if ( !pawn.IsValid() )
		{
			pawn = new CollapsePlayer();
			pawn.MakePawnOf( client );
			pawn.Respawn();
		}
		else
		{
			pawn.MakePawnOf( client );
		}

		PersistenceSystem.Load( pawn );

		base.ClientJoined( client );
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		if ( pawn is CollapsePlayer player && player.Bedroll.IsValid() )
		{
			player.Position = player.Bedroll.Position + Vector3.Up * 10f;
			return;
		}

		base.MoveToSpawnpoint( pawn );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		if ( client.Pawn is CollapsePlayer player )
		{
			PersistenceSystem.Save( player );
		}

		InventorySystem.ClientDisconnected( client );

		base.ClientDisconnect( client, reason );
	}

	public override void PostLevelLoaded()
	{
		Game.WorldEntity.Tags.Add( "world" );

		{
			var spawner = new PickupSpawner();
			spawner.SetType<WoodPickup>();
			spawner.MaxPickups = 100;
			spawner.MaxPickupsPerSpawn = 20;
			spawner.MaxPickupsPerSpawn = 80;
			spawner.Interval = 60f;
		}

		{
			var spawner = new PickupSpawner();
			spawner.SetType<StonePickup>();
			spawner.MaxPickups = 70;
			spawner.MaxPickupsPerSpawn = 20;
			spawner.MaxPickupsPerSpawn = 60;
			spawner.Interval = 120f;
		}

		{
			var spawner = new PickupSpawner();
			spawner.SetType<MetalOrePickup>();
			spawner.MaxPickups = 50;
			spawner.MaxPickupsPerSpawn = 20;
			spawner.MaxPickupsPerSpawn = 60;
			spawner.Interval = 180f;
		}

		{
			var spawner = new PickupSpawner();
			spawner.SetType<PlantFiberPickup>();
			spawner.MaxPickups = 40;
			spawner.MaxPickupsPerSpawn = 20;
			spawner.MaxPickupsPerSpawn = 60;
			spawner.Interval = 90f;
		}

		NextDespawnItems = 30f;
		HasLoadedWorld = true;
		NextAutoSave = 60f;

		base.PostLevelLoaded();
	}
	

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( HasLoadedWorld && NextAutoSave && ShouldAutoSave )
		{
			Log.Info( "[Collapse] Saving world..." );
			PersistenceSystem.SaveAll();
			NextAutoSave = 60f;
		}

		if ( HasLoadedWorld && NextDespawnItems )
		{
			var items = All.OfType<ItemEntity>();

			foreach ( var item in items )
			{
				if ( item.TimeSinceSpawned >= 1800f )
				{
					item.Delete();
				}
			}

			NextDespawnItems = 30f;
		}
	}

	[Event.Client.Frame]
	private void OnFrame()
	{
		Camera?.Update();
	}
		static async Task<string> SpawnPackageModel( string packageName, Vector3 pos, Rotation rotation, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Model || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var model = package.GetMeta( "PrimaryAsset", "models/dev/error.vmdl" );
		var mins = package.GetMeta( "RenderMins", Vector3.Zero );
		var maxs = package.GetMeta( "RenderMaxs", Vector3.Zero );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		return model;
	}

	[ConCmd.Server( "spawn" )]
	public static async Task Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn as Player;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

		//
		// Does this look like a package?
		//
		if ( modelname.Count( x => x == '.' ) == 1 && !modelname.EndsWith( ".vmdl", System.StringComparison.OrdinalIgnoreCase ) && !modelname.EndsWith( ".vmdl_c", System.StringComparison.OrdinalIgnoreCase ) )
		{
			modelname = await SpawnPackageModel( modelname, tr.EndPosition, modelRotation, owner as Entity );
			if ( modelname == null )
				return;
		}

		var model = Model.Load( modelname );
		if ( model == null || model.IsError )
			return;

		var ent = new Prop
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		// If there's no physics model, create a simple OBB
		if ( !ent.PhysicsBody.IsValid() )
		{
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs );
		}
	}
}
