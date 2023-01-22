using Sandbox;
using Editor;

namespace NxtStudio.Collapse;

[HammerEntity, Category( "Roofs" ), Library( "house_roof" ), Title( "Roof" )]
[SupportsSolid]

public partial class Roof : ModelEntity
{
	/// <summary>
	/// Help text in hammer
	/// </summary>
	[Property( Title = "Visible" ) ]
	public bool Visible { get; set; } = true;


	public override void Spawn()
	{

		Tags.Add( "solid", "roof" );

		EnableHitboxes = true;
		EnableAllCollisions = true;

        SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		base.Spawn();
	}

	protected override void OnTagAdded( string tag )
	{
		Log.Info($"ADICIONEI A TAG {tag}");
		
		base.OnTagAdded( tag );
	}
	
	protected override void OnTagRemoved( string tag )
	{
		Log.Info($"REMOVI A TAG {tag}");
		
		base.OnTagRemoved( tag );
	}

	public override void StartTouch( Entity cl )
	{
		Log.Info($"TOUCH {cl}");
	}
}
