﻿using Sandbox;
using System;

namespace Facepunch.Forsaken;

[Library]
public partial class Projectile : ModelEntity
{
	[Net, Predicted] public string ExplosionEffect { get; set; } = "";
	[Net, Predicted] public string LaunchSoundName { get; set; } = null;
	[Net, Predicted] public string FollowEffect { get; set; } = "";
	[Net, Predicted] public string TrailEffect { get; set; } = "";
	[Net, Predicted] public string HitSound { get; set; } = "";
	[Net, Predicted] public string ModelName { get; set; } = "";

	public Action<Projectile, TraceResult> Callback { get; private set; }
	public bool PlayFlybySounds { get; set; } = false;
	public RealTimeUntil CanHitTime { get; set; } = 0.1f;
	public ProjectileSimulator Simulator { get; set; }
	public float? LifeTime { get; set; }
	public string Attachment { get; set; } = null;
	public Entity Attacker { get; set; } = null;
	public bool ExplodeOnDestroy { get; set; } = true;
	public Entity IgnoreEntity { get; set; }
	public float Gravity { get; set; } = 10f;
	public float Radius { get; set; } = 8f;
	public bool FaceDirection { get; set; } = false;
	public Vector3 StartPosition { get; private set; }
	public bool Debug { get; set; } = false;

	protected float GravityModifier { get; set; }
	protected RealTimeUntil DestroyTime { get; set; }
	protected SceneObject ModelEntity { get; set; }
	protected Vector3 InitialVelocity { get; set; }
	protected Sound LaunchSound { get; set; }
	protected Particles Follower { get; set; }
	protected Particles Trail { get; set; }

	public void Initialize( Vector3 start, Vector3 velocity, float radius, Action<Projectile, TraceResult> callback = null )
	{
		Initialize( start, velocity, callback );
		Radius = radius;
	}

	public void Initialize( Vector3 start, Vector3 velocity, Action<Projectile, TraceResult> callback = null )
	{
		if ( LifeTime.HasValue )
		{
			DestroyTime = LifeTime.Value;
		}

		if ( Simulator != null && Simulator.IsValid() )
		{
			Simulator?.Add( this );
			Owner = Simulator.Owner;
		}

		InitialVelocity = velocity;
		StartPosition = start;
		EnableDrawing = false;
		Velocity = velocity;
		Callback = callback;
		Position = start;

		if ( IsClientOnly )
		{
			using ( Prediction.Off() )
			{
				CreateEffects();
			}
		}
	}

	public override void Spawn()
	{
		Predictable = true;

		base.Spawn();
	}

    public override void ClientSpawn()
    {
		// We only want to create effects if we don't have a client proxy.
		if ( !HasClientProxy() )
        {
			CreateEffects();
		}

        base.ClientSpawn();
    }

	public virtual void CreateEffects()
    {
		if ( !string.IsNullOrEmpty( TrailEffect ) )
		{
			Trail = Particles.Create( TrailEffect, this );

			if ( !string.IsNullOrEmpty( Attachment ) )
				Trail.SetEntityAttachment( 0, this, Attachment );
			else
				Trail.SetEntity( 0, this );
		}

		if ( !string.IsNullOrEmpty( FollowEffect ) )
		{
			Follower = Particles.Create( FollowEffect, this );
		}

		if ( !string.IsNullOrEmpty( LaunchSoundName ) )
			LaunchSound = PlaySound( LaunchSoundName );
	}

    public virtual void Simulate()
    {
		if ( FaceDirection )
        {
			Rotation = Rotation.LookAt( Velocity.Normal );
        }

		if ( Debug )
        {
			DebugOverlay.Sphere( Position, Radius, Game.IsClient ? Color.Blue : Color.Red );
        }

		var newPosition = GetTargetPosition();

		var trace = Trace.Ray( Position, newPosition )
			.UseHitboxes()
			.Size( Radius )
			.Ignore( this )
			.Ignore( IgnoreEntity )
			.Run();

		Position = trace.EndPosition;

		if ( LifeTime.HasValue && DestroyTime )
		{
			if ( ExplodeOnDestroy )
			{
				PlayHitEffects( Vector3.Zero );
				Callback?.Invoke( this, trace );
			}

			Delete();

			return;
		}

		if ( HasHitTarget( trace ) )
		{
			PlayHitEffects( trace.Normal );
			Callback?.Invoke( this, trace );
			Delete();
		}
	}

	public bool HasClientProxy()
    {
		return !IsClientOnly && Owner.IsValid() && Owner.IsLocalPawn;

	}

	protected virtual bool HasHitTarget( TraceResult trace )
	{
		return (trace.Hit && CanHitTime) || trace.StartedSolid;
	}

	protected virtual Vector3 GetTargetPosition()
	{
		var newPosition = Position;
		newPosition += Velocity * Time.Delta;

		GravityModifier += Gravity;
		newPosition -= new Vector3( 0f, 0f, GravityModifier * Time.Delta );

		return newPosition;
	} 

	[ClientRpc]
	protected virtual void PlayHitEffects( Vector3 normal )
    {
		if ( HasClientProxy() )
        {
			// We don't want to play hit effects if we have a client proxy.
			return;
        }

		if ( !string.IsNullOrEmpty( ExplosionEffect ) )
		{
			var explosion = Particles.Create( ExplosionEffect );

			if ( explosion != null )
			{
				explosion.SetPosition( 0, Position );
				explosion.SetForward( 0, normal );
			}
		}

		if ( !string.IsNullOrEmpty( HitSound ) )
		{
			Sound.FromWorld( HitSound, Position );
		}
	}

	[Event.Tick.Client]
	protected virtual void ClientTick()
	{
		if ( ModelEntity.IsValid() )
		{
			ModelEntity.Transform = Transform;
		}
	}

	[Event.Tick.Server]
	protected virtual void ServerTick()
	{
		if ( !Simulator.IsValid() )
		{
			Simulate();
		}
	}

    protected override void OnDestroy()
	{
		Simulator?.Remove( this );
		RemoveEffects();
		base.OnDestroy();
	}

	private void RemoveEffects()
	{
		ModelEntity?.Delete();
		LaunchSound.Stop();
		Follower?.Destroy();
		Trail?.Destroy();
		Trail = null;
	}
}
