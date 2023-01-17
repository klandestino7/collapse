using Sandbox;

namespace Facepunch.Collapse;

public interface IHeatEmitter : IValid
{
	public float EmissionRadius { get; }
	public float HeatToEmit { get; }
	public Vector3 Position { get; }
}
