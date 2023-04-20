using Editor;
using Sandbox;

namespace NxtStudio.Collapse
{
	[Title( "Poison Zone" )]
	[Description( "Applies poison damage over time to any players inside this zone." )]
	[Category( "Triggers" )]
	[HammerEntity]
	public partial class PoisonZone : BaseTrigger
	{
		[Property] public float PoisonDamagePerSecond { get; set; } = 5f;
		[Property] public float PoisonProtectionThreshold { get; set; } = 10f;
		[Property] public bool DamageScalesToCenter { get; set; } = true;

		public override void StartTouch( Entity other )
		{
			if ( other is CollapsePlayer player )
			{
				player.InsideZones.Add( this );
			}

			base.StartTouch( other );
		}

		public override void EndTouch( Entity other )
		{
			if ( other is CollapsePlayer player )
			{
				player.InsideZones.Remove( this );
			}

			base.EndTouch( other );
		}
	}
}
