using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NxtStudio.Collapse;

public class AvoidanceBehavior : EntityComponent
{
	public HashSet<string> ObstacleTags { get; set; } = new();

	public float MaxAcceleration { get; set; } = 15f;
	public float MinimumDistance { get; set; } = 80f;
	public float MaxStandableAngle { get; set; } = 20f;

	public float MainWhiskerLength { get; set; } = 45f;
	public float SideWhiskerLength { get; set; } = 25f;
	public float SideWhiskerAngle { get; set; } = 45f;

	private SteeringComponent Steering { get; set; }

	protected override void OnActivate()
	{
		Steering = Entity.Components.GetOrCreate<SteeringComponent>();
	}

	public Vector3 GetSteering()
	{
		var velocity = Entity.Velocity.WithZ( 0f );

		if ( velocity.Length > 0.005f )
			return GetSteering( velocity );
		else
			return GetSteering( Entity.Rotation.Forward );
	}

	public Vector3 GetSteering( Vector3 facingDir )
	{
		var acceleration = Vector3.Zero;
		TraceResult trace;

		if ( !FindObstacle( facingDir, out trace ) )
		{
			return acceleration;
		}

		var ahead = Entity.Position + facingDir.Normal * MinimumDistance;
		var avoidanceForce = (trace.EndPosition - ahead).Normal * MaxAcceleration;
		return avoidanceForce;
	}

	private bool FindObstacle( Vector3 facingDir, out TraceResult result )
	{
		facingDir = facingDir.Normal;

		var dirs = new Vector3[3];
		dirs[0] = facingDir;

		var orientation = SteeringComponent.VectorToOrientation( facingDir );
		dirs[1] = SteeringComponent.OrientationToVector( orientation + SideWhiskerAngle.DegreeToRadian() );
		dirs[2] = SteeringComponent.OrientationToVector( orientation - SideWhiskerAngle.DegreeToRadian() );

		return CastWhiskers( dirs, out result );
	}

	private bool CastWhiskers( Vector3[] dirs, out TraceResult result )
	{
		result = default;

		var obstacleTagsArray = ObstacleTags.ToArray();
		var didHitAnything = false;
		var origin = Entity.Position + Vector3.Up * 12f;

		for ( int i = 0; i < dirs.Length; i++ )
		{
			var distance = i == 0 ? MainWhiskerLength : SideWhiskerLength;
			var trace = Trace.Ray( origin, origin + dirs[i] * distance )
				.WorldAndEntities()
				.WithoutTags( "passplayers" )
				.WithAnyTags( "solid" )
				.WithAnyTags( obstacleTagsArray )
				.Ignore( Entity )
				.Size( 8f )
				.Run();

			if ( trace.Hit )
			{
				if ( NPC.Debug )
					DebugOverlay.Line( origin, origin + dirs[i] * distance, Entity.Velocity.IsNearZeroLength ? Color.Cyan : Color.Green );

				if ( trace.Normal.Angle( Vector3.Up ) > MaxStandableAngle )
				{
					didHitAnything = true;
					result = trace;
					break;
				}
			}
			else
			{
				if ( NPC.Debug )
					DebugOverlay.Line( origin, origin + dirs[i] * distance, Color.Red );
			}
		}

		return didHitAnything;
	}
}
