using System;

namespace Facepunch.Collapse;

public partial class WoodPickup : ResourcePickup
{
	public override string ModelPath => "models/resources/tree_stump.vmdl";
	public override Type ItemType => typeof( WoodItem );
	public override int StackSize => 50;

	public override string GetContextName()
	{
		return "Tree Stump";
	}
}
