using Sandbox;

namespace Facepunch.Collapse;

public partial class HotbarContainer : InventoryContainer
{
	public HotbarContainer() : base()
	{
		SetSlotLimit( 8 );
	}

	public override InventoryContainer GetTransferTarget( InventoryItem item )
	{
		if ( Entity is CollapsePlayer player )
		{
			return UI.Storage.Current.IsOpen ? UI.Storage.Current.Container : CollapsePlayer.Me.Backpack;
		}

		return base.GetTransferTarget( item );
	}
}
