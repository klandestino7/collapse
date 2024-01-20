using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public sealed class PlayerCrossAim : Component
{
	public Rotation lookToRotation;
	[Property] public GameObject Body { get; set; }
	public Vector2 cursorDirection { get; private set; }

	public Vector3 m_v3PawnCursorDir { get; private set ; }

    private bool playerOnMovement { get; set;}

	public bool aiming { get; private set;}
	protected override void OnUpdate()
	{
        var runButtonPressed = Input.Down( "run" );
		var attackButtonPressed = Input.Down( "attack1" );
		var aimButtonPressed = Input.Down( "attack2" );

        var cursorTraceStartPos = new Vector2( Body.Transform.Position.x, Body.Transform.Position.y );

        Log.Info( "CURSOR :: " + Mouse.Position );
        Gizmo.Draw.Line( cursorTraceStartPos, Mouse.Position );
	}
    

	protected override void OnFixedUpdate()
	{
        if ( IsProxy )
			return;

		var cc = GameObject.Components.Get<CharacterController>();
		playerOnMovement = cc.Velocity.Length >= 1f;
	}
}
