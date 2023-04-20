namespace NxtStudio.Collapse;

public interface ILimitedSpawner
{
	public Vector3 Position { get; set; }
	public Rotation Rotation { get; set; }
	public void Despawn();
}
