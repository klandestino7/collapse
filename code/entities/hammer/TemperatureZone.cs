using Editor;
using Sandbox;

namespace NxtStudio.Collapse
{
	[Title( "Temperature Zone")]
	[Description( "Modifies the temperature of players who enter this area." )]
	[Category( "Triggers" )]
	[HammerEntity]
	public partial class TemperatureZone : BaseTrigger
	{
		[Property] public float Temperature { get; set; } = 0f;

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
