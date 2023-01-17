using Sandbox;

namespace Facepunch.Collapse;

public struct ConsumableEffect
{
	[Property]
	public ConsumableType Target { get; set; }

	[Property]
	public float Amount { get; set; }

	[Property]
	public float Duration { get; set; }
}

public enum ConsumableType
{
	Calories,
	Hydration,
	Health,
	Stamina
}
