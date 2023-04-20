using Sandbox;
using System.Linq;

namespace NxtStudio.Collapse;

[Title( "Doorway" )]
[Description( "Can have a door placed inside. Must be placed on a foundation." )]
[Icon( "textures/ui/doorway.png" )]
[ItemCost( "wood", 50 )]
public partial class Doorway : UpgradableStructure
{
	public override float MaxHealth => 250f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/structures/doorway.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Tags.Add( "hammer", "solid", "wall", "doorway" );
	}

	public override string GetContextName()
	{
		return $"Doorway ({Health.CeilToInt()}HP)";
	}

	public override bool CanConnectTo( Socket socket )
	{
		return !FindInSphere( socket.Position, 32f )
			.OfType<Structure>()
			.Where( s => !s.Equals( this ) )
			.Where( s => s.Tags.Has( "wall" ) )
			.Any();
	}

	public override void OnNewModel( Model model )
	{
		if ( Game.IsServer || IsClientOnly )
		{
			var socket = AddSocket( "center" );
			socket.ConnectAny.Add( "wall" );
			socket.Tags.Add( "doorway" );

			socket = AddSocket( "door" );
			socket.ConnectAny.Add( "door" );
			socket.Tags.Add( "doorway" );
		}

		base.OnNewModel( model );
	}

	public override void OnKilled()
	{
		base.OnKilled();

		// Let's destroy any doors attached to the doorway.
		foreach ( var socket in Sockets )
		{
			if ( socket.Connection.IsValid() )
			{
				var entity = socket.Connection.Parent as SingleDoor;

				if ( entity.IsValid() )
				{
					Breakables.Break( entity );

					entity.OnKilled();
					entity.Delete();
				}
			}
		}
	}
}
