using Sandbox;

namespace NxtStudio.Collapse;

public interface IDamageable : IValid
{
	public void TakeDamage( DamageInfo info );
	public float Health { get; }
	public float MaxHealth { get; }
	public LifeState LifeState { get; }
}
