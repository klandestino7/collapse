using Editor;
using Sandbox;

namespace NxtStudio.Collapse
{
	[Title( "Named Zone")]
	[Description( "Displays the name of this zone when a player enters the area." )]
	[Category( "Triggers" )]
	[HammerEntity]
	public partial class NamedZone : BaseTrigger
	{
		[Property] public string DisplayName { get; set; } = "Untitled Zone";

		public override void StartTouch( Entity other )
		{
			if ( other is CollapsePlayer player )
			{
				UI.Hud.ShowZoneName( To.Single( player ), DisplayName );
			}
			
			base.StartTouch( other );
		}
	}
}
