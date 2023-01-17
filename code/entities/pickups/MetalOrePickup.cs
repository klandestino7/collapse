using System;

namespace Facepunch.Collapse;

public partial class MetalOrePickup : ResourcePickup
{
	public override string ModelPath => "models/resources/metal_vein.vmdl";
	public override Type ItemType => typeof( MetalOre );
	public override int StackSize => 15;

	public override string GetContextName()
	{
		return "Metal Vein";
	}
}
