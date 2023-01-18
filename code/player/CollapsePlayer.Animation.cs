using Sandbox;

namespace NxtStudio.Collapse;

public partial class CollapsePlayer
{
	private Weapon LastWeaponEntity { get; set; }

	protected void SimulateAnimation()
	{
		Rotation rotation;

		// where should we be rotated to
		var turnSpeed = 0.02f;

		// If we're a bot, spin us around 180 degrees.
		if (Client.IsBot)
			rotation = ViewAngles.WithYaw(ViewAngles.yaw + 180f).ToRotation();
		else
			rotation = ViewAngles.ToRotation();

		var runButtonPressed = Input.Down(InputButton.Run);
		var attackButtonPressed = Input.Down(InputButton.PrimaryAttack);
		var aimButtonPressed = Input.Down(InputButton.SecondaryAttack);

		var animHelper = new CitizenAnimationHelper( this );

		var aimActived = false;

		if ( runButtonPressed && !aimButtonPressed )
		{
			var newRotation = Rotation.LookAt(InputDirection, Vector3.Up);
			Rotation = Rotation.Lerp(Rotation, newRotation, Time.Delta * 10f);
			aimActived = false;
		} 
		else if ( runButtonPressed && aimButtonPressed )
		{
			rotation = Rotation.LookAt(InputDirection, Vector3.Up);
			aimActived = false;
		}
		else if ( aimButtonPressed && !runButtonPressed )
		{
			Rotation = Rotation.Lerp(Rotation, rotation, Time.Delta * 10f);
			aimActived = true;
		}
		else if (Velocity.Length >= 1f )
		{
			var newRotation = Rotation.LookAt(InputDirection, Vector3.Up);
			Rotation = Rotation.Lerp(Rotation, newRotation, Time.Delta * 10f);
			aimActived = false;
		}
		else if (Velocity.Length <= 1f && attackButtonPressed )
		{
			Rotation = Rotation.Lerp(Rotation, rotation, Time.Delta * 10f);
			aimActived = false;
		}


		animHelper.WithWishVelocity( Controller.WishVelocity );
		animHelper.WithVelocity( Velocity );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, Controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );

		animHelper.VoiceLevel = (Game.IsClient && Client.IsValid()) ? Client.Voice.LastHeard < 0.5f ? Client.Voice.CurrentLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = GroundEntity != null;

		animHelper.IsSitting = Controller.HasTag( "sitting" );
		animHelper.IsNoclipping = Controller.HasTag( "noclip" );
		animHelper.IsClimbing = Controller.HasTag( "climbing" );
		animHelper.IsSwimming = false;
		animHelper.IsWeaponLowered = false;

		if ( Controller.HasEvent( "jump" ) ) animHelper.TriggerJump();

		if ( ActiveChild != LastWeaponEntity ) animHelper.TriggerDeploy();

		if ( ActiveChild is Weapon weapon )
		{
			weapon.SimulateAnimator( animHelper );
		}
		else
		{
			var holdType = CitizenAnimationHelper.HoldTypes.None;

			if (aimButtonPressed)
			{
				holdType = CitizenAnimationHelper.HoldTypes.Punch;
			}

			animHelper.HoldType = holdType;
		}


		LastWeaponEntity = ActiveChild as Weapon;
	}
}
