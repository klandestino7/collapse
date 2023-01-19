using Sandbox;

namespace NxtStudio.Collapse;

public partial class CollapsePlayer
{
	public Rotation lastRotation;
	public Vector3 cursorDirection { get; private set; }
	public Vector3 cameraPosition { get; private set; }


	public Vector3 WorldAimPosition { get; private set; }


    public void CrossaimSimulation()
    {

		var runButtonPressed = Input.Down(InputButton.Run);
		var attackButtonPressed = Input.Down(InputButton.PrimaryAttack);
		var aimButtonPressed = Input.Down(InputButton.SecondaryAttack);

		var aimActived = false;

		// if ( runButtonPressed && !aimButtonPressed )
		// {
		// 	var newRotation = Rotation.LookAt(InputDirection, Vector3.Up);
		// 	Rotation = Rotation.Lerp(Rotation, newRotation, Time.Delta * 10f);
		// 	aimActived = false;
		// } 
		// else if ( runButtonPressed && aimButtonPressed )
		// {
		// 	rotation = Rotation.LookAt(InputDirection, Vector3.Up);
		// 	aimActived = false;
		// }
		// else if ( aimButtonPressed && !runButtonPressed )
		// {
		// 	Rotation = Rotation.Lerp(Rotation, rotation, Time.Delta * 10f);
		// 	aimActived = true;
		// }
		// else if (Velocity.Length >= 1f )
		// {
		// 	var newRotation = Rotation.LookAt(InputDirection, Vector3.Up);
		// 	Rotation = Rotation.Lerp(Rotation, newRotation, Time.Delta * 10f);
		// 	aimActived = false;
		// }
		// else if (Velocity.Length <= 1f && attackButtonPressed )
		// {
		// 	Rotation = Rotation.Lerp(Rotation, rotation, Time.Delta * 10f);
		// 	aimActived = false;
		// }


		var isSimulating = Prediction.CurrentHost.IsValid();

		// # Fazer o player virar pare essa posição;
		if (Game.IsClient && isSimulating)
		{
			cursorDirection = Screen.GetDirection( Screen.Size * Cursor );
			cameraPosition = Camera.Position;
		}

		var startPosition = cameraPosition;
		var endPosition = cameraPosition + cursorDirection * 1000f;

		var player = CollapsePlayer.Me;

		var cursor = Trace.Ray(startPosition, endPosition)
			.WithAnyTags("world")
			.WithoutTags("wall")
			.Radius(2)
			.Run();
		
		DebugOverlay.Line(EyePosition, cursor.EndPosition, Color.Yellow, 0);

		var playerOnMovement = Velocity.Length >= 1f;

		var animHelper = new CitizenAnimationHelper( this );

		if ( runButtonPressed )
		{
			animHelper.IsWeaponLowered = false;
		}

		// ANDANDO SEM MIRAR
		if ( playerOnMovement && !aimButtonPressed )
		{
			var newRotation = Rotation.LookAt(InputDirection, Vector3.Up);
			lastRotation = Rotation.Lerp(Rotation, newRotation, Time.Delta * 10f);
			
			animHelper.WithLookAt( EyePosition , 1.0f, 1.0f, 0.5f ); // Mirando
		}

		// MIRANDO E PARADO
		if (aimButtonPressed && !playerOnMovement ) 
		{
			var newRotation = Rotation.LookAt( EyeRotation.Backward );

			lastRotation = Rotation.Lerp( EyeRotation, newRotation, Time.Delta * 1f );
		

			// lastRotation = Rotation.LookAt( EyeRotation.Backward );

			animHelper.WithLookAt( cursor.EndPosition, 1.0f, 1.0f, 0.5f ); // Mirando

			// var idealRotation = Rotation.LookAt( EyeRotation.Forward.WithZ( 0 ), Vector3.Up );

			// DoRotation( idealRotation );
			// DoWalk( idealRotation );

			// Vector3 aimPos = cursorDirection + ( EyePosition * 200);

	
		}

		// CLICAR E PARADO
		if ( attackButtonPressed && !playerOnMovement ) 
		{
			var newRotation = Rotation.LookAt(cursorDirection, Vector3.Up);
			lastRotation = Rotation.Lerp( EyeRotation, newRotation, Time.Delta * 10f);
		}

		// ANDANDO E MIRANDO
		if ( aimButtonPressed && playerOnMovement && !runButtonPressed) 
		{
			var newRotation = Rotation.LookAt( cursorDirection , Vector3.Up);
			lastRotation = Rotation.Lerp( EyeRotation, newRotation, Time.Delta * 10f);

			animHelper.WithLookAt( cursor.EndPosition , 1.0f, 1.0f, 0.5f ); // Mirando
		}

		WorldAimPosition = cursor.EndPosition;
		Rotation = lastRotation; 

		// animHelper.AimAngle = Rotation.LookAt(InputDirection, cursor.EndPosition);
		// animHelper.AimAngle = Rotation;


		// 		// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
		// if ( Game.IsServer && Input.Pressed( InputButton.PrimaryAttack ) )
		// {
		// 	var ragdoll = new ModelEntity();
		// 	ragdoll.SetModel( "models/citizen/citizen.vmdl" );
		// 	ragdoll.Position = cursor.EndPosition;
		// 	ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
		// 	ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		// 	// ragdoll.PhysicsGroup.Velocity = Rotation.Forward * 1000;
		// }
    }


	
	public virtual void DoRotation( Rotation idealRotation )
	{
		float turnSpeed = 0.01f;
		// If we're moving, rotate to our ideal rotation
		Rotation = Rotation.Slerp( Rotation, idealRotation, Controller.WishVelocity.Length * Time.Delta * turnSpeed );

		// Clamp the foot rotation to within 120 degrees of the ideal rotation
		Rotation = Rotation.Clamp( idealRotation, 0, out var change );
	}
}
