using Sandbox;

namespace NxtStudio.Collapse;

[Library( "weapon_pump_shotgun" )]
public partial class PumpShotgun : ProjectileWeapon<CrossbowBoltProjectile>
{
	public override string ProjectileData => IsSlugAmmo() ? "slug" : "buckshot";
	public override string MuzzleFlashEffect => "particles/pistol_muzzleflash.vpcf";
	public override string DamageType => "bullet";
	public override float PrimaryRate => 1f;
	public override float SecondaryRate => 1f;
	public override CitizenAnimationHelper.HoldTypes HoldType => CitizenAnimationHelper.HoldTypes.Shotgun;
	public override float InheritVelocity => 0f;
	public override string ReloadSoundName => "shotgun_load";
	public override int ProjectileCount => IsSlugAmmo() ? 1 : 8;
	public override float ReloadTime => 1f;
	public override float Spread => IsSlugAmmo() ? 0.05f : 1f;

	public override void AttackPrimary()
	{
		if ( !TakeAmmo( 1 ) )
		{
			PlaySound( "gun.dryfire" );
			return;
		}

		PlayAttackAnimation();
		ShootEffects();
		PlaySound( $"shotgun1_shoot" );
		ApplyRecoil();

		base.AttackPrimary();
	}

	protected override void OnReloadFinish()
	{
		Game.AssertServer();

		IsReloading = false;
		TimeSinceReload = 0f;

		ResetReloading();

		if ( AmmoClip >= ClipSize )
			return;

		if ( Owner is not CollapsePlayer player )
			return;

		if ( !WeaponItem.IsValid() )
			return;

		if ( !WeaponItem.AmmoDefinition.IsValid() )
			return;

		if ( !UnlimitedAmmo )
		{
			var ammo = player.TakeAmmo( WeaponItem.AmmoDefinition.UniqueId, 1 );
			if ( ammo == 0 ) return;
			AmmoClip += 1;
		}
		else
		{
			AmmoClip += 1;
		}

		if ( AmmoClip < ClipSize )
		{
			Reload();
		}
	}

	protected override void ShootEffects()
	{
		var position = GetMuzzlePosition();

		if ( position.HasValue )
			CreateLightSource( position.Value, Color.White, 300f, 0.1f, Time.Delta );

		base.ShootEffects();
	}

	private bool IsSlugAmmo()
	{
		if ( !WeaponItem.IsValid() )
			return false;

		if ( !WeaponItem.AmmoDefinition.IsValid() )
			return false;

		return WeaponItem.AmmoDefinition.Tags.Contains( "slug" );
	}
}
