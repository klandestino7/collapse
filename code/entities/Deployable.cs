﻿using Sandbox;
using System.Linq;

namespace NxtStudio.Collapse;

public partial class Deployable : ModelEntity
{
	public static ModelEntity Ghost { get; private set; }

	public static ModelEntity GetOrCreateGhost( Model model )
	{
		if ( !Ghost.IsValid() || Ghost.Model != model )
		{
			ClearGhost();

			Ghost = new ModelEntity
			{
				EnableShadowCasting = false,
				EnableShadowReceive = false,
				EnableAllCollisions = false,
				Transmit = TransmitType.Never,
				Model = model
			};

			Ghost.SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
			Ghost.SetMaterialOverride( Material.Load( "materials/blueprint.vmat" ) );
		}

		return Ghost;
	}

	public static void ClearGhost()
	{
		Ghost?.Delete();
		Ghost = null;
	}

	public static bool IsCollidingWithWorld( ModelEntity entity )
	{
		var testPosition = entity.Position + Vector3.Up * 4f;
		var collision = Trace.Body( entity.PhysicsBody, entity.Transform.WithPosition( testPosition ), testPosition )
			.WithAnyTags( "nobuild", "solid", "world" )
			.Run();

		return (collision.Hit || collision.StartedSolid);
	}

	public virtual void OnPlacedByPlayer( CollapsePlayer player, TraceResult trace )
	{

	}
}
