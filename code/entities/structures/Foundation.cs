﻿using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace NxtStudio.Collapse;

[Title( "Foundation" )]
[Description( "The most fundamental building block. Walls, doors and windows can be attached to it." )]
[Icon( "textures/ui/foundation.png" )]
[ItemCost( "wood", 100 )]
public partial class Foundation : UpgradableStructure
{
	protected override int StoneUpgradeCost => 100;
	protected override int MetalUpgradeCost => 50;

	public override bool RequiresSocket => false;
	public override bool ShouldRotate => false;
	public override float MaxHealth => 250f;

	public Stockpile Stockpile { get; private set; }

	public void PropagateStockpile( Stockpile stockpile )
	{
		if ( Stockpile == stockpile )
			return;

		Stockpile = stockpile;

		foreach ( var socket in Sockets )
		{
			if ( socket.Connection.IsValid() )
			{
				var other = socket.Connection.Parent as Foundation;

				if ( other.IsValid() )
				{
					other.PropagateStockpile( stockpile );
				}
			}
		}
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/structures/foundation.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Tags.Add( "hammer", "solid", "foundation" );
	}

	public override void OnPlacedByPlayer( CollapsePlayer player )
	{
		var pickups = FindInBox( WorldSpaceBounds ).OfType<ResourcePickup>();

		foreach ( var pickup in pickups )
		{
			pickup.Delete();
		}

		base.OnPlacedByPlayer( player );
	}

	public override string GetContextName()
	{
		return $"Foundation ({Health.CeilToInt()}HP)";
	}

	public override void OnConnected( Socket ours, Socket theirs )
	{
		if ( theirs.Parent is Foundation other )
		{
			Stockpile = other.Stockpile;
		}
	}

	public override bool IsValidPlacement( Vector3 target, Vector3 normal )
	{
		var nerarbyFoundations = FindInSphere( target, PrivilegeRange )
			.OfType<Foundation>()
			.Where( s => !s.Equals( this ) );

		if ( nerarbyFoundations.Any() )
			return false;

		return base.IsValidPlacement( target, normal );
	}

	public override void OnNewModel( Model model )
	{
		if ( Game.IsServer || IsClientOnly )
		{
			AddFoundationSocket( "forward", "backward" );
			AddFoundationSocket( "backward", "forward" );
			AddFoundationSocket( "left", "right" );
			AddFoundationSocket( "right", "left" );

			AddWallSocket( "forward" );
			AddWallSocket( "backward" );
			AddWallSocket( "left" );
			AddWallSocket( "right" );
		}

		base.OnNewModel( model );
	}

	private void AddFoundationSocket( string direction, string connectorDirection )
	{
		var socket = AddSocket( direction );
		socket.ConnectAny.Add( "foundation" );
		socket.ConnectAll.Add( connectorDirection );
		socket.Tags.Add( "foundation", direction );
	}

	private void AddWallSocket( string attachmentName )
	{
		var socket = AddSocket( attachmentName );
		socket.Tags.Add( "foundation", "wall" );
	}
}
