using Sandbox;

namespace NxtStudio.Collapse;

public partial class CollapsePlayer
{
	public Rotation lookToRotation;
	public Vector3 cursorDirection { get; private set; }

	public Vector3 m_v3PawnCursorDir { get; private set ; }

	public bool aiming { get; private set;}

    public void CrossaimSimulation()
    {
		var runButtonPressed = Input.Down(InputButton.Run);
		var attackButtonPressed = Input.Down(InputButton.PrimaryAttack);
		var aimButtonPressed = Input.Down(InputButton.SecondaryAttack);
		var playerOnMovement = Velocity.Length >= 1f;

		var isSimulating = Prediction.CurrentHost.IsValid();

		var player = CollapsePlayer.Me;

		// # Fazer o player virar pare essa posição;
		if (Game.IsClient && isSimulating)
		{
			cursorDirection = Screen.GetDirection( Screen.Size * Cursor );
		}

		var cursorTraceStartPos = Camera.Position;
		var cursorTraceEndPos = cursorTraceStartPos + (cursorDirection * 1000.0f);

		var cursorTrace = Trace.Ray(cursorTraceStartPos, cursorTraceEndPos)
			.WithAnyTags("world")
			.WithoutTags("wall")
			.Radius(8)
			.Run();

		bool inCombat = aimButtonPressed; // !Input.MouseDelta.IsNearZeroLength()

		Vector3 cursorTraceHitPos = cursorTrace.EndPosition; // cursorTrace.Value;

		Vector3 aimrayDirToCursor = (cursorTraceHitPos - this.AimRay.Position).Normal;

		bool walkingInCombat = playerOnMovement && inCombat;
		var walkingNotInCombat = playerOnMovement; // && !aimButtonPressed;

		Rotation lookAtRotation = Rotation.LookAt(InputDirection, Vector3.Up); 
		Vector3 lookAtPos = this.EyePosition;
		
		aiming = false;

		if (inCombat)
		{
			lookAtRotation = Rotation.LookAt(aimrayDirToCursor.WithZ(0f));
			lookAtPos = cursorTraceHitPos;
		}

		var animHelper = new CitizenAnimationHelper( this );

		if ( runButtonPressed )
		{
			animHelper.IsWeaponLowered = false;
		}

		if ( walkingNotInCombat )
		{
		}

		// MIRANDO E PARADO
		if (aimButtonPressed && !playerOnMovement ) 
		{
			aiming = true;
		}

		// CLICAR E PARADO
		if ( attackButtonPressed && !playerOnMovement ) 
		{
		}

		// ANDANDO E MIRANDO
		if ( aimButtonPressed && playerOnMovement && !runButtonPressed) 
		{
			aiming = true;
		}

		lookAtRotation = Rotation.Lerp(Rotation, lookAtRotation, Time.Delta * 5f);

		if ( inCombat /* && (m_v3PawnCursorDir == null || m_v3PawnCursorDir.Distance(aimrayDirToCursor) >= 0.01f) */)
		{
			Velocity = Velocity + aimrayDirToCursor * 10.0f;

			m_v3PawnCursorDir = aimrayDirToCursor;
		}
		else
		{
			m_v3PawnCursorDir = this.AimRay.Forward;
		}

		animHelper.WithVelocity( Velocity );
		animHelper.WithLookAt( lookAtPos, 1.0f, 1.0f, 0.5f );

		Rotation = lookAtRotation;
    }
}
