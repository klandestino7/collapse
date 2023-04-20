using System;

namespace NxtStudio.Collapse;

public partial class StonePickup : ResourcePickup
{
	public override string GatherSound => "rummage.stone";
	public override string ModelPath => "models/resources/stone_pile.vmdl";
	public override Type ItemType => typeof( StoneItem );
	public override int StackSize => 25;

	public override string GetContextName()
	{
		return "Stone Pile";
	}
}
