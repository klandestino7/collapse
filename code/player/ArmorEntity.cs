using Sandbox;

namespace NxtStudio.Collapse;

public partial class ArmorEntity : ModelEntity
{
	public ArmorItem Item { get; set; }

	public override void ClientSpawn()
	{
		if ( Parent is CollapsePlayer player && player.IsLocalPawn )
		{
			CollapsePlayer.AddObscuredGlow( this );
		}

		base.ClientSpawn();
	}

	public override void Spawn()
	{
		Tags.Add( "armor" );

		base.Spawn();
	}
}
