using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public sealed class CursorAction : Component
{
	[Property] float Speed { get; set; } = 1f;
	[Property] float LerpSpeed { get; set; } = 10f;
	[Property] float Range { get; set; } = 100f;

	[Sync] 
	public Vector3 ScreenPosition { get; set; }

	public int UpNext = 1;

	float hspeed = 0f;
	TimeSince timeSinceLastDrop = 0f;

	TimeSince timeSinceLastMouseMove = 0f;

	protected override void OnUpdate()
	{

		ScreenPosition = new Vector3( Mouse.Position.x, Mouse.Position.y, 0f);

		// var tr = Scene.Trace.Ray(Scene.Camera.ScreenPixelToRay( Mouse.Position ), Scene.Camera.ZFar);
		// Gizmo.Draw.Line();

		if ( Input.AnalogLook.yaw != 0f )
		{
			timeSinceLastMouseMove = 0f;
		}

		if ( timeSinceLastMouseMove < 0.5f )
		{
			hspeed = hspeed.LerpTo( Input.AnalogLook.yaw * Speed * 10f, Time.Delta * LerpSpeed * 5f );
		}
		else if ( Input.Down( "Left" ) )
		{
			hspeed = hspeed.LerpTo( Speed, Time.Delta * LerpSpeed );
		}
		else if ( Input.Down( "Right" ) )
		{
			hspeed = hspeed.LerpTo( -Speed, Time.Delta * LerpSpeed );
		}
		else
		{
			hspeed = hspeed.LerpTo( 0f, Time.Delta * LerpSpeed );
		}

		var y = Transform.Position.y;
		y += Time.Delta * hspeed;
		y = Math.Clamp( y, -Range, Range );
		Transform.Position = Transform.Position.WithY( y );

	}
}
