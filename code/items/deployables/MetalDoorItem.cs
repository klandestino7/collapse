using Sandbox;
using System;

namespace NxtStudio.Collapse;

public class MetalDoorItem : SingleDoorItem
{
	public override Type Deployable => typeof( MetalDoor );
	public override string UniqueId => "metal_door";
	public override string Icon => "textures/items/metal_door.png";
	public override string Name => "Metal Door";
}
