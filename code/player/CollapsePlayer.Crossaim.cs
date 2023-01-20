using Sandbox;

namespace NxtStudio.Collapse;

public partial class CollapsePlayer
{
	public Rotation lookToRotation;
	public Vector3 cursorDirection { get; private set; }

	public Vector3 WorldAimPosition { get; private set; }

    public void CrossaimSimulation()
    {
		var runButtonPressed = Input.Down(InputButton.Run);
		var attackButtonPressed = Input.Down(InputButton.PrimaryAttack);
		var aimButtonPressed = Input.Down(InputButton.SecondaryAttack);
		var playerOnMovement = Velocity.Length >= 1f;

		var aimActived = false;

		var isSimulating = Prediction.CurrentHost.IsValid();

		var player = CollapsePlayer.Me;

		// # Fazer o player virar pare essa posição;
		if (Game.IsClient && isSimulating)
		{
			cursorDirection = Screen.GetDirection( Screen.Size * Cursor );
		}

		var startPosition = Camera.Position;
		var endPosition = startPosition + cursorDirection * 1000f;

		var cursor = Trace.Ray(startPosition, endPosition)
			.WithAnyTags("world")
			.WithoutTags("wall")
			.Radius(8)
			.Run();

		var pawnPosToCursorDirection = (cursor.EndPosition - this.Position).Normal;

		var justWalking = playerOnMovement && !aimButtonPressed;

		var lookAtRotation = justWalking ? Rotation.LookAt(InputDirection, Vector3.Up) : Rotation.LookAt(pawnPosToCursorDirection);

		var animHelperWithLookAt = justWalking ? EyePosition : cursor.EndPosition;

		var animHelper = new CitizenAnimationHelper( this );

		if ( runButtonPressed )
		{
			animHelper.IsWeaponLowered = false;
		}

		if ( justWalking )
		{
			lookToRotation = Rotation.Lerp(Rotation, lookAtRotation, Time.Delta * 2f);
		}

		// MIRANDO E PARADO
		if (aimButtonPressed && !playerOnMovement ) 
		{
			lookToRotation = Rotation.Lerp(Rotation, lookAtRotation, Time.Delta * 2f);
		}

		// CLICAR E PARADO
		if ( attackButtonPressed && !playerOnMovement ) 
		{
			lookToRotation = Rotation.Lerp(Rotation, lookAtRotation, Time.Delta * 2f);
		}

		// ANDANDO E MIRANDO
		if ( aimButtonPressed && playerOnMovement && !runButtonPressed) 
		{
			lookToRotation = Rotation.Lerp(Rotation, lookAtRotation, Time.Delta * 2f);
		}

		animHelper.WithLookAt( animHelperWithLookAt , 1.0f, 1.0f, 0.5f ); 

		Rotation = lookToRotation;

		WorldAimPosition = cursor.EndPosition;
    }
}
