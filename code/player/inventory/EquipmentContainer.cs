using Sandbox;

namespace NxtStudio.Collapse;

public partial class EquipmentContainer : InventoryContainer
{
	public EquipmentContainer() : base()
	{
		SetSlotLimit( 4 );
	}

	public override InventoryContainer GetTransferTarget( InventoryItem item )
	{
		if ( Entity is CollapsePlayer player )
		{
			return UI.Storage.Current.IsOpen ? UI.Storage.Current.Container : CollapsePlayer.Me.Backpack;
		}

		return base.GetTransferTarget( item );
	}

	public override bool CanGiveItem( ushort slot, InventoryItem item )
	{
		if ( item is not ArmorItem armor )
			return false;

		if ( armor.ArmorSlot == ArmorSlot.Head )
			return slot == 0;

		if ( armor.ArmorSlot == ArmorSlot.Chest )
			return slot == 1;

		if ( armor.ArmorSlot == ArmorSlot.Legs )
			return slot == 2;

		if ( armor.ArmorSlot == ArmorSlot.Feet )
			return slot == 3;

		return false;
	}
}
