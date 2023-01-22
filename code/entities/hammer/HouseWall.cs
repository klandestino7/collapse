using Sandbox;
using Editor;

namespace NxtStudio.Collapse;

[HammerEntity, Category( "Walls" ), Library( "house_wall" ), Title( "HouseWall" )]
[SupportsSolid]

public partial class HouseWall : ModelEntity
{
	/// <summary>
	/// Help text in hammer
	/// </summary>
	[Property( Title = "Visible" ) ]
	public bool Visible { get; set; } = true;


	public override void Spawn()
	{
		Tags.Add( "solid", "wall" );

		EnableHitboxes = true;
		EnableAllCollisions = true;

        SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		base.Spawn();
	}


	public override void StartTouch( Entity cl )
	{
		Log.Info($"TOUCH {cl}");
	}
}
