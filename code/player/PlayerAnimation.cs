using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public sealed class PlayerAnimation : Component
{
	// private Weapon LastWeaponEntity { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }

	public Vector3 WishVelocity { get; private set; }

	[Sync] 
	public Angles EyeAngles { get; set; }

	[Sync]
	public bool IsRunning { get; set; }

	protected override void OnUpdate()
	{
		Rotation rotation;
        Rotation bodyRotation = Body.Transform.Rotation;

		var cam = GameObject.Components.Get<PlayerCamera>( ) ;
		EyeAngles = cam.EyeAngles;
		// Eye input
		if ( !IsProxy )
		{
			IsRunning = Input.Down( "Run" );
		}

		var cc = GameObject.Components.Get<CharacterController>();
        
        var aimButtonPressed = Input.Down( "attack2" );

        float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			var v = cc.Velocity.WithZ( 0 );

			targetAngle = Rotation.LookAt( v, Vector3.Up );

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			if ( rotateDifference > 50.0f || cc.Velocity.Length > 10.0f )
			{
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 2.0f );
			}

            if ( aimButtonPressed )
            {
				// Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 2.0f );
            }
		}

        if ( AnimationHelper is not null )
        {
            AnimationHelper.WithVelocity( cc.Velocity );
            AnimationHelper.WithWishVelocity( WishVelocity );
            AnimationHelper.IsGrounded = cc.IsOnGround;
            AnimationHelper.FootShuffle = rotateDifference;
            AnimationHelper.AimAngle = bodyRotation;
            AnimationHelper.WithLook( EyeAngles.Forward, 1, 1, 1.0f );
            AnimationHelper.MoveStyle = IsRunning ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
        }
	}

	protected override void OnFixedUpdate()
	{
		
	}
}
