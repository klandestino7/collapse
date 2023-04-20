using Sandbox;

namespace NxtStudio.Collapse;

public partial class CollapsePlayer
{
	public PlayerCorpse Ragdoll { get; set; }

	[ClientRpc]
	private void BecomeRagdollOnClient( Vector3 force, int forceBone )
	{
		var ragdoll = new PlayerCorpse
		{
			Position = Position,
			Rotation = Rotation
		};

		ragdoll.CopyFrom( this );
		ragdoll.ApplyAbsoluteImpulse( Velocity );
		ragdoll.ApplyForceToBone( force, forceBone );

		Ragdoll = ragdoll;
	}

	private void BecomeRagdollOnServer( Vector3 force, int forceBone )
	{
		var ragdoll = new PlayerCorpse
		{
			Position = Position,
			Rotation = Rotation
		};

		ragdoll.CopyFrom( this );
		ragdoll.ApplyAbsoluteImpulse( Velocity );
		ragdoll.ApplyForceToBone( force, forceBone );

		Ragdoll = ragdoll;
	}
}
