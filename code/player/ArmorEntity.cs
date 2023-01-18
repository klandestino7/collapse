﻿using Sandbox;

namespace NxtStudio.Collapse;

public partial class ArmorEntity : ModelEntity
{
	public ArmorItem Item { get; set; }

	public override void ClientSpawn()
	{
		if ( Parent is ForsakenPlayer player && player.IsLocalPawn )
		{
			ForsakenPlayer.AddObscuredGlow( this );
		}

		base.ClientSpawn();
	}
}
