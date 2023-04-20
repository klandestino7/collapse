using Sandbox;
using System.Linq;

namespace NxtStudio.Collapse;

[Title( "Wall" )]
[Description( "Prevents anything getting in or out. Must be attached to a foundation." )]
[Icon( "textures/ui/wall.png" )]
[ItemCost( "wood", 50 )]
public partial class Wall : UpgradableStructure
{
	public override float MaxHealth => 250f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/structures/wall.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Tags.Add( "hammer", "solid", "wall" );
	}

	public override string GetContextName()
	{
		return $"Wall ({Health.CeilToInt()}HP)";
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
			socket.Tags.Add( "wall" );
		}

		base.OnNewModel( model );
	}
}
