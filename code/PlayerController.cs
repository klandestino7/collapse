using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public sealed class PlayerController : Component
{

	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );

	public Vector3 WishVelocity { get; private set; }


	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	[Property] public bool FirstPerson { get; set; }

	[Sync]
	public Angles EyeAngles { get; set; }

	protected float WheelSpeed => 30f;
	protected Vector2 CameraDistance => new( 200, 1000 );
	protected Vector2 PitchClamp => new( 25, 70 );

	float OrbitDistance = 200f;
	float TargetOrbitDistance = 400f;
	Angles OrbitAngles = Angles.Zero;

	SkinnedModelRenderer playerBody { get; set; }

	[Sync]
	public bool IsRunning { get; set; }
	protected override void OnEnabled()
	{
		base.OnEnabled();

		if ( IsProxy )
			return;

		var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();
		if ( cam is not null )
		{
			var ee = cam.Transform.Rotation.Angles();
			ee.roll = 0;
			EyeAngles = ee;
		}


	}

	protected override void OnStart()
	{
		playerBody = Scene.GetAllComponents<SkinnedModelRenderer>().FirstOrDefault();
		Log.Info( " playerBody " + playerBody );
	}

	protected static Vector3 IntersectPlane( Vector3 pos, Vector3 dir, float z )
	{
		float a = (z - pos.z) / dir.z;
		return new( dir.x * a + pos.x, dir.y * a + pos.y, z );
	}

	protected static Rotation LookAt( Vector3 targetPosition, Vector3 position )
	{
		var targetDelta = (targetPosition - position);
		var direction = targetDelta.Normal;

		return Rotation.From( new Angles(
			((float)Math.Asin( direction.z )).RadianToDegree() * -1.0f,
			((float)Math.Atan2( direction.y, direction.x )).RadianToDegree(),
			0.0f ) );
	}


	protected override void OnUpdate()
	{
		var cc = GameObject.Components.Get<CharacterController>();

		// Eye input
		if ( !IsProxy )
		{
			var ee = EyeAngles;
			ee += Input.AnalogLook * 0.5f;
			ee.roll = 0;
			EyeAngles = ee;

			var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();

			Vector3 targetPos;

			if ( playerBody != null )
			{
				cam.Transform.Position = playerBody.Transform.Position;

				cam.Transform.Position += Vector3.Up * (playerBody.Bounds.Center.z * playerBody.Transform.Scale);
				cam.Transform.Rotation = Rotation.From( OrbitAngles );

				targetPos = cam.Transform.Position + cam.Transform.Rotation.Backward * OrbitDistance;
				cam.Transform.Position = targetPos;
			}

			IsRunning = Input.Down( "Run" );
		}


		if ( cc is null ) return;

		float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			var v = cc.Velocity.WithZ( 0 );

			if ( v.Length > 10.0f )
			{
				targetAngle = Rotation.LookAt( v, Vector3.Up );
			}

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			if ( rotateDifference > 50.0f || cc.Velocity.Length > 10.0f )
			{
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 2.0f );
			}
		}

		if ( AnimationHelper is not null )
		{
			AnimationHelper.WithVelocity( cc.Velocity );
			AnimationHelper.WithWishVelocity( WishVelocity );
			AnimationHelper.IsGrounded = cc.IsOnGround;
			AnimationHelper.FootShuffle = rotateDifference;
			AnimationHelper.WithLook( EyeAngles.Forward, 1, 1, 1.0f );
			AnimationHelper.MoveStyle = IsRunning ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
		}

	}

	[Broadcast]
	public void OnJump( float floatValue, string dataString, object[] objects, Vector3 position )
	{
		AnimationHelper?.TriggerJump();
	}

	float fJumps;

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		BuildWishVelocity();

		var cc = GameObject.Components.Get<CharacterController>();


		CameraInputsAction();

		if ( cc.IsOnGround && Input.Down( "Jump" ) )
		{
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;
			//if ( Duck.IsActive )
			//	flMul *= 0.8f;

			cc.Punch( Vector3.Up * flMul * flGroundFactor );
			//	cc.IsOnGround = false;

			OnJump( fJumps, "Hello", new object[] { Time.Now.ToString(), 43.0f }, Vector3.Random );

			fJumps += 1.0f;
		}

		if ( cc.IsOnGround )
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
			cc.Accelerate( WishVelocity );
			cc.ApplyFriction( 4.0f );
		}
		else
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
			cc.Accelerate( WishVelocity.ClampLength( 50 ) );
			cc.ApplyFriction( 0.1f );
		}

		cc.Move();

		if ( !cc.IsOnGround )
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}
	}

	public void BuildWishVelocity()
	{
		var rot = EyeAngles.ToRotation();

		WishVelocity = rot * Input.AnalogMove;
		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Down( "Run" ) ) WishVelocity *= 320.0f;
		else WishVelocity *= 110.0f;
	}


	public void CameraInputsAction()
	{

		var wheel = Input.MouseWheel;

		if ( wheel.y != 0 )
		{
			TargetOrbitDistance -= wheel.y * WheelSpeed;
			TargetOrbitDistance = TargetOrbitDistance.Clamp( CameraDistance.x, CameraDistance.y );
		}

		OrbitDistance = OrbitDistance.LerpTo( TargetOrbitDistance, Time.Delta * 10f );

		if ( Input.UsingController || ( Input.Down( "Walk" ) ) )
		{
			OrbitAngles.yaw += Input.AnalogLook.yaw * 5f;
			OrbitAngles.pitch += Input.AnalogLook.pitch * 5f;
			OrbitAngles = OrbitAngles.Normal;

			EyeAngles = OrbitAngles.WithPitch( 0f );

			// Entity.ViewAngles = OrbitAngles.WithPitch( 0f );
		}
		else
		{
			//			var direction = Screen.GetDirection( Mouse.Position, Camera.FieldOfView, Camera.Rotation, Screen.Size );
			//			var hitPos = IntersectPlane( Camera.Position, direction, Entity.EyePosition.z );

			//			Entity.ViewAngles = (hitPos - Entity.EyePosition).EulerAngles;
		}

		OrbitAngles.pitch = OrbitAngles.pitch.Clamp( PitchClamp.x, PitchClamp.y );

		//		Entity.InputDirection = Input.AnalogMove;
	}
}
