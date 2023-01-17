using Sandbox;

namespace Facepunch.Collapse;

[Library]
public partial class CrossbowBoltProjectile : Projectile
{
	public override void CreateEffects()
    {
		base.CreateEffects();
		Trail?.SetPosition( 6, Color.Red * 255f );
	}
}
