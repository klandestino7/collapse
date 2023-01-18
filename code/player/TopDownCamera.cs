// using Sandbox;

// namespace NxtStudio.Collapse;

// public partial class TopDownCamera
// {
//     public float ZoomLevel { get; set; }
//     public Vector3 LookAt { get; set; }
    
// 	public float Height { get; set; } = 650f;
// 	public float MoveSpeed { get; set; } = 20f;

// 	private float Scale { get; set; } = 1.5f;

//     public float MinZoom = 200f;
//     public float MaxZoom = 900f;

//     public Vector3 camOffset { get; set; }


// 	// private bool shiftToggle = false;

// 	// public virtual float CameraHeight => 400;
// 	// public virtual float CameraAngle => 65;

// 	// private Angles ang;
// 	// private Angles tarAng;
// 	// private Vector3 camOffset;
// 	// private Vector3 camOffsetTarget;

// 	// public Rotation Rotation;
// 	// public Vector3 Position;

// 	// public bool CameraShift { get; set; }

// 	// public void BuildInput()
// 	// {
// 	// 	var pawn = Game.LocalPawn;

// 	// 	if ( pawn == null || pawn is not CollapsePlayer player )
// 	// 	{
// 	// 		return;
// 	// 	}

// 	// 	Angles angles;

// 	// 	// handle look input
// 	// 	if ( !Input.UsingController )
// 	// 	{

// 	// 		// if ( pawn.IsValid() )
// 	// 		// {
// 	// 		// 	if ( !Settings.ViewshiftToggle && Input.Down( InputButton.Run ) )
// 	// 		// 	{
// 	// 		// 		CameraShift = true;
// 	// 		// 	}
// 	// 		// 	else if ( Settings.ViewshiftToggle && Input.Pressed( InputButton.Run ) )
// 	// 		// 	{
// 	// 		// 		shiftToggle = !shiftToggle;
// 	// 		// 	}
// 	// 		// 	else
// 	// 		// 	{
// 	// 		// 		CameraShift = false;
// 	// 		// 	}
// 	// 		// }
// 	// 		// else
// 	// 		// {
// 	// 			CameraShift = false;
// 	// 		// }

// 	// 		var direction = Screen.GetDirection( new Vector2( Mouse.Position.x, Mouse.Position.y ), 70, Rotation, Screen.Size );
// 	// 		var HitPosition = LinePlaneIntersectionWithHeight( Position, direction, player.EyePosition.z - 20 );

// 	// 		// since we got our cursor in world space because of the plane intersect above, we need to set it for the crosshair
// 	// 		var mouse = HitPosition.ToScreen();
// 	// 		// Crosshair.UpdateMouse( new Vector2( mouse.x * Screen.Width, mouse.y * Screen.Height ) );

// 	// 		//trace from camera into mouse direction, essentially gets the world location of the mouse
// 	// 		var targetTrace = Trace.Ray( Position, Position + (direction * 1000) )
// 	// 			.UseHitboxes()
// 	// 			.EntitiesOnly()
// 	// 			.Size( 1 )
// 	// 			.Ignore( player )
// 	// 			.Run();

// 	// 		// aim assist when pointing on a player
// 	// 		// if ( targetTrace.Hit && targetTrace.Entity is CollapsePlayer )
// 	// 		// {
// 	// 		// 	if ( Debug.Camera )
// 	// 		// 		DebugOverlay.Line( player.EyePosition, targetTrace.Entity.AimRay.Position + (Vector3.Down * 20), Color.Red, 0, true );
// 	// 		// 	angles = (targetTrace.Entity.AimRay.Position + (Vector3.Down * 20) - (player.EyePosition - (Vector3.Up * 20))).EulerAngles;
// 	// 		// }
// 	// 		// else
// 	// 		// {
// 	// 			angles = (HitPosition - (player.EyePosition - (Vector3.Up * 20))).EulerAngles;
// 	// 		// }

// 	// 	}
// 	// 	else
// 	// 	{
// 	// 		// shift on clicking in joystick
// 	// 		if (  Input.Down( InputButton.View ) )
// 	// 		{
// 	// 			CameraShift = true;
// 	// 		}
// 	// 		else if (  Input.Pressed( InputButton.View ) )
// 	// 		{
// 	// 			shiftToggle = !shiftToggle;
// 	// 		}
// 	// 		else
// 	// 		{
// 	// 			CameraShift = false;
// 	// 		}

// 	// 		if ( MathF.Abs( Input.AnalogLook.pitch ) + MathF.Abs( Input.AnalogLook.yaw ) > 0 )
// 	// 		{
// 	// 			//var angle = MathF.Atan2(input.AnalogLook.pitch, input.AnalogLook.yaw).RadianToDegree();
// 	// 			Angles newDir = new Vector3( Input.AnalogLook.pitch / 1.5f * -1.0f, Input.AnalogLook.yaw / 1.5f, 0 ).EulerAngles;
// 	// 			ControllerLookInput = new Vector2( Input.AnalogLook.yaw, -Input.AnalogLook.pitch ).Normal;
// 	// 			angles = newDir;
// 	// 		}
// 	// 		else
// 	// 		{
// 	// 			// not moving joystick, don't update angles
// 	// 			return;
// 	// 		}

// 	// 	}

// 	// 	tarAng = angles;
// 	// 	ang = Angles.Lerp( ang, tarAng, 24 * Time.Delta );

// 	// 	CollapsePlayer.Me.ViewAngles = ang;
// 	// }

// 	// private Vector2 ControllerLookInput { get; set; } = Vector2.Zero;

// 	// public void Update()
// 	// {
// 	// 	var pawn = CollapsePlayer.Me;
// 	// 	// if ( Game.LocalPawn is not CollapsePlayer pawn )
// 	// 	// 	return;

// 	// 	var _pos = pawn.EyePosition + (Vector3.Down * 20); // relative to pawn EyePosition
// 	// 	_pos += Vector3.Up * CameraHeight; // add camera height
// 	// 									   // why didn't we just do this with Rotation.LookAt????
// 	// 									   // [DOC] answer: cause we (I) wanted a fixed/clearly defined angle
// 	// 	_pos -= Vector3.Forward * (float)(CameraHeight / Math.Tan( MathX.DegreeToRadian( CameraAngle ) )); // move camera back

// 	// 	float mouseShiftFactor = 0.3f;//Sniper
// 	// 	var wep = pawn.ActiveChild as Weapon;
// 	// 	if ( wep is not null )
// 	// 	{
// 	// 		mouseShiftFactor = 0.5f;
// 	// 	}

// 	// 	float MouseX = Mouse.Position.x.Clamp( 0, Screen.Size.x );
// 	// 	float MouseY = Mouse.Position.y.Clamp( 0, Screen.Size.y );

// 	// 	camOffsetTarget = CameraShift ||  shiftToggle
// 	// 		? Input.UsingController
// 	// 			? (Vector3.Left * (ControllerLookInput.x * Screen.Size.x / 2) * mouseShiftFactor) + (Vector3.Forward * (ControllerLookInput.y * Screen.Size.y / 2) * mouseShiftFactor)
// 	// 			: (Vector3.Left * -((MouseX - (Screen.Size.x / 2)) * mouseShiftFactor)) + (Vector3.Forward * -((MouseY - (Screen.Size.y / 2)) * mouseShiftFactor))
// 	// 		: Vector3.Zero;
// 	// 	camOffset = Vector3.Lerp( camOffset, camOffsetTarget, Time.Delta * 8f );

// 	// 	_pos += camOffset;

// 	// 	Position = _pos;

// 	// 	Rotation = Rotation.FromAxis( Vector3.Left, CameraAngle );

// 	// 	Sound.Listener = new()
// 	// 	{
// 	// 		Position = pawn.IsValid() ? pawn.EyePosition : Position,
// 	// 		Rotation = Rotation
// 	// 	};


// 	// 	// debug stuff for aim location
// 	// 	// if ( Debug.Camera )
// 	// 	// {
// 	// 		var direction = Screen.GetDirection( new Vector2( Mouse.Position.x, Mouse.Position.y ), 70, Rotation, Screen.Size );
// 	// 		var HitPosition = LinePlaneIntersectionWithHeight( Position, direction, pawn.EyePosition.z );
// 	// 		// 
// 	// 		DebugOverlay.ScreenText( $"Pos {Position}", new Vector2( 300, 300 ), 2, Color.Green );
// 	// 		DebugOverlay.ScreenText( $"Dir {direction}", new Vector2( 300, 300 ), 3, Color.Green );
// 	// 		DebugOverlay.ScreenText( $"HitPos {HitPosition}", new Vector2( 300, 300 ), 4, Color.Green );
// 	// 		// 
// 	// 		var Distance = HitPosition - pawn.EyePosition;
// 	// 		// 
// 	// 		DebugOverlay.Line( pawn.EyePosition, pawn.EyePosition + Distance, Color.Green, 0, false );

// 	// 		// TEMP CROSSHAIR
// 	// 		DebugOverlay.Sphere( HitPosition, 5, Color.Green, Time.Delta, false );
// 	// 	// }


// 	// 	Camera.Position = Position;
// 	// 	Camera.Rotation = Rotation;

// 	// 	Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 50f );
// 	// 	Camera.FirstPersonViewer = null;
// 	// }

// 	// // resolve line plane intersect for mouse input
// 	// public static Vector3 LinePlaneIntersectionWithHeight( Vector3 pos, Vector3 dir, float z )
// 	// {
// 	// 	float px, py, pz;

// 	// 	//solve for temp, zpos = (zdir) * (temp) + (initialZpos)
// 	// 	float temp = (z - pos.z) / dir.z;

// 	// 	//plug in and solve for Px and Py
// 	// 	px = (dir.x * temp) + pos.x;
// 	// 	py = (dir.y * temp) + pos.y;
// 	// 	pz = z;
// 	// 	return new Vector3( px, py, pz );
// 	// }

//     public void Update()
//     {
        
// 		var pawn = CollapsePlayer.Me;

//         if ( pawn.IsValid() ) 
//         {

// 			ZoomLevel += Input.MouseWheel * Time.Delta * 8f;
// 			ZoomLevel = ZoomLevel.Clamp( 0f, 1f );

//             // Camera.ZNear = 50f;
// 			// Camera.ZFar = 900f;

// 			Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 30f );

// 			// var velocity = Vector3.Zero;
// 			var panSpeed = MaxZoom - (MaxZoom * ZoomLevel * 0.6f);

//             // if ( pawn.Velocity.Length > 0 )
//             // {
//             //     velocity = pawn.Velocity * panSpeed;
//             // }

// 			var lookAtPosition = (LookAt + 0.1f * Time.Delta);

//             var center = pawn.Position + Vector3.Up * 50;

// 			lookAtPosition.x = center.x;
// 			lookAtPosition.y = center.y;

// 			LookAt = center;

// 			Vector3 eyePos;

// 			eyePos = LookAt + Vector3.Backward * (MinZoom - (MinZoom * ZoomLevel * 0.6f));
// 			eyePos += Vector3.Left * (MinZoom - (MinZoom * ZoomLevel * 0.6f));
// 			eyePos += Vector3.Up * (MaxZoom - (MaxZoom * ZoomLevel * 0.6f));

//             var camPosition = Camera.Position.LerpTo( eyePos, Time.Delta * 2f );
            
//             var tr = Trace.Ray(LookAt, eyePos)
//                 .WithAnyTags("world")
//                 .WithoutTags("wall")
//                 .Radius(2)
//                 .Run();

// 			camOffset = LookAt - eyePos;
            
// 			Camera.Rotation = Rotation.Slerp( Camera.Rotation, Rotation.LookAt( camOffset, Vector3.Up ), Time.Delta * 8f );

// 			Sound.Listener = new Transform()
// 			{
// 				Position = lookAtPosition,
// 				Rotation = Camera.Rotation
// 			};

// 		    DebugOverlay.Line(LookAt, tr.EndPosition, Color.Blue, 0);

// 			Camera.Position = tr.EndPosition;
// 			Camera.FirstPersonViewer = null;
//         }
//     }
// }
