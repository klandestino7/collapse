using Sandbox;
using Sandbox.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NxtStudio.Collapse;

public partial class CollapseGame : GameManager
{
	public static CollapseGame Entity => Current as CollapseGame;
	public static string UniqueSaveId => Entity?.InternalSaveId ?? string.Empty;

	[ConVar.Replicated( "fsk.isometric" )]
	public static bool Isometric { get; set; } = true;

	[ConVar.Server( "fsk.autosave", Saved = true )]
	public static bool ShouldAutoSave { get; set; } = true;

	[ConVar.Server( "fsk.pvp" )]
	public static bool EnablePvP { get; set; } = true;

	[ConVar.Server( "fsk.pve" )]
	public static bool EnablePvE { get; set; } = true;

	private TimeUntil NextDespawnItems { get; set; }
	private TimeUntil NextAutoSave { get; set; }
	private IsometricCamera IsometricCamera { get; set; }
	private TopDownCamera TopDownCamera { get; set; }
	private bool HasLoadedWorld { get; set; }

	[Net] private string InternalSaveId { get; set; }
	private ScreenEffects PostProcessing { get; set; }

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

		IsometricCamera = new();
		TopDownCamera = new();

		PostProcessing = new();
		Camera.Main.RemoveAllHooks();
		Camera.Main.AddHook( PostProcessing );

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

	public override bool CanHearPlayerVoice( IClient source, IClient receiver )
	{
		if ( !source.IsValid() || !receiver.IsValid() ) return false;

		var a = source.Pawn as CollapsePlayer;
		var b = source.Pawn as CollapsePlayer;

		if ( !a.IsValid() || !b.IsValid() ) return false;

		return a.Position.Distance( b.Position ) <= 2000f;
	}

	public override void OnVoicePlayed( IClient cl )
	{
		cl.Voice.WantsStereo = false;
		base.OnVoicePlayed( cl );
	}

	public override void PostLevelLoaded()
	{
		Game.WorldEntity.Tags.Add( "world" );

		{
			var spawner = new LimitedSpawner();
			spawner.SetType<WoodPickup>();
			spawner.UseNavMesh = true;
			spawner.MaxTotal = 400;
			spawner.MinPerSpawn = 200;
			spawner.MaxPerSpawn = 300;
			spawner.Interval = 120f;
		}

		{
			var spawner = new LimitedSpawner();
			spawner.SetType<StonePickup>();
			spawner.UseNavMesh = true;
			spawner.MaxTotal = 300;
			spawner.MinPerSpawn = 100;
			spawner.MaxPerSpawn = 200;
			spawner.Interval = 180f;
		}

		{
			var spawner = new LimitedSpawner();
			spawner.SetType<MetalOrePickup>();
			spawner.UseNavMesh = true;
			spawner.MaxTotal = 200;
			spawner.MinPerSpawn = 100;
			spawner.MaxPerSpawn = 150;
			spawner.Interval = 300f;
		}

		{
			var spawner = new LimitedSpawner();
			spawner.SetType<PlantFiberPickup>();
			spawner.UseNavMesh = true;
			spawner.MaxTotal = 250;
			spawner.MinPerSpawn = 150;
			spawner.MaxPerSpawn = 200;
			spawner.Interval = 120f;
		}

		{
			var spawner = new LimitedSpawner();
			spawner.SetType<Deer>();
			spawner.UseNavMesh = true;
			spawner.MaxTotal = 20;
			spawner.MinPerSpawn = 1;
			spawner.MaxPerSpawn = 10;
			spawner.Interval = 120f;
		}

		{
			var spawner = new LimitedSpawner();
			spawner.SetType<Undead>();
			spawner.OnSpawned = ( e ) => (e as Undead)?.RiseFromGround();
			spawner.SpawnNearPlayers = true;
			spawner.TimeOfDayStart = 19.5f;
			spawner.TimeOfDayEnd = 7f;
			spawner.UseNavMesh = true;
			spawner.MaxTotal = 20;
			spawner.MinPerSpawn = 10;
			spawner.MaxPerSpawn = 20;
			spawner.Interval = 10f;
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

		InternalSaveId = PersistenceSystem.UniqueId;
	}

	[Event.Client.Frame]
	private void OnFrame()
	{
		if ( Isometric )
			IsometricCamera?.Update();
		else
			TopDownCamera?.Update();

		if ( Game.LocalPawn is not CollapsePlayer player )
			return;

		var pp = PostProcessing;

		pp.ChromaticAberration.Scale = 0.05f;
		pp.ChromaticAberration.Offset = Vector3.Zero;

		pp.Brightness = 0.99f;
		pp.Contrast = 1.01f;
		pp.Sharpen = 0.25f;

		var healthScale = (0.2f / player.MaxHealth) * player.Health;

		if ( player.LifeState == LifeState.Alive )
			pp.Saturation = 0.8f + healthScale;
		else
			pp.Saturation = 0f;

		pp.Vignette.Intensity = 0.8f - healthScale * 4f;
		pp.Vignette.Color = Color.Red.WithAlpha( 0.1f );
		pp.Vignette.Smoothness = 0.9f;
		pp.Vignette.Roundness = 0.3f;

		var sum = ScreenShake.List.OfType<ScreenShake.Random>().Sum( s => (1f - s.Progress) );

		pp.Pixelation = 0.02f * sum;
		pp.ChromaticAberration.Scale += (0.05f * sum);
	}
}
