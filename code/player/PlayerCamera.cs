using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Numerics;

public sealed class PlayerCamera : Component
{
	[Property] public float WheelSpeed => 30f;
	[Property] public Vector2 CameraDistance => new( 200, 1000 );
	[Property] public Vector2 PitchClamp => new( 25, 70 );
	[Property] public SkinnedModelRenderer playerBody { get; set; }

	[Sync]
	public Angles EyeAngles { get; set; }

	float OrbitDistance = 200f;
	float TargetOrbitDistance = 400f;
	Angles OrbitAngles = Angles.Zero;

	protected override void OnUpdate()
	{
		var cc = GameObject.Components.Get<CharacterController>();

		if ( !IsProxy )
		{
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
		}
	}

	protected override void OnFixedUpdate()
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
		}

		OrbitAngles.pitch = OrbitAngles.pitch.Clamp( PitchClamp.x, PitchClamp.y );
	}
}
