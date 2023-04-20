using Sandbox;
using System.Collections.Generic;
using System.IO;

namespace NxtStudio.Collapse;

public abstract partial class UpgradableStructure : Structure
{
	public override float MaxHealth => GetMaxHealth();

	protected virtual int StoneUpgradeCost => 200;
	protected virtual int MetalUpgradeCost => 100;

	private ContextAction StoneUpgradeAction { get; set; }
	private ContextAction MetalUpgradeAction { get; set; }

	[Net] public StructureMaterial Material { get; private set; }

	[ConCmd.Server]
	public static void DestroyStructure()
	{
		if ( ConsoleSystem.Caller.Pawn is CollapsePlayer player )
		{
			var tr = Trace.Ray( player.CameraPosition, player.CameraPosition + player.CursorDirection * 10000f )
				.EntitiesOnly()
				.Run();

			if ( tr.Entity is Structure structure )
			{
				structure.TakeDamage( DamageInfo.Generic( 10000f ) );
			}
		}
	}

	public UpgradableStructure() : base()
	{
		StoneUpgradeAction = new( "upgrade.stone", $"Upgrade", "textures/items/stone.png" );
		StoneUpgradeAction.SetCondition( p =>
		{
			return new ContextAction.Availability
			{
				IsAvailable = p.HasItems<StoneItem>( StoneUpgradeCost ),
				Message = $"{StoneUpgradeCost} x Stone"
			};
		} );

		MetalUpgradeAction = new( "upgrade.metal", $"Upgrade", "textures/items/metal_fragments.png" );
		MetalUpgradeAction.SetCondition( p =>
		{
			return new ContextAction.Availability
			{
				IsAvailable = p.HasItems<MetalFragments>( MetalUpgradeCost ),
				Message = $"{MetalUpgradeCost} x Metal Fragments"
			};
		} );
	}

	public override void SerializeState( BinaryWriter writer )
	{
		base.SerializeState( writer );

		writer.Write( (byte)Material );
	}

	public override void DeserializeState( BinaryReader reader )
	{
		base.DeserializeState( reader );

		Material = (StructureMaterial)reader.ReadByte();
	}

	public override void BeforeStateLoaded()
	{
		UpdateMaterial();

		base.BeforeStateLoaded();
	}

	public override IEnumerable<ContextAction> GetSecondaryActions( CollapsePlayer player )
	{
		var hotbarItem = player.GetActiveHotbarItem();

		if ( hotbarItem is HammerItem && player.HasPrivilegeAt( Position ) )
		{
			if ( Material == StructureMaterial.Wood )
			{
				if ( player.HasItems<MetalFragments>( MetalUpgradeCost ) )
				{
					yield return StoneUpgradeAction;
				}
			}
		}
	}

	public override ContextAction GetPrimaryAction( CollapsePlayer player )
	{
		var hotbarItem = player.GetActiveHotbarItem();

		if ( hotbarItem is HammerItem && player.HasPrivilegeAt( Position ) )
		{
			if ( Material == StructureMaterial.Wood )
			{
				if ( player.HasItems<MetalFragments>( MetalUpgradeCost ) )
					return MetalUpgradeAction;
				else
					return StoneUpgradeAction;
			}
			else if ( Material == StructureMaterial.Stone )
			{
				return MetalUpgradeAction;
			}
		}

		return default;
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( info.HasTag( "melee" ) )
		{
			using ( Prediction.Off() )
			{
				if ( Material == StructureMaterial.Wood )
					PlaySound( "melee.hitwood" );
				else if ( Material == StructureMaterial.Stone )
					PlaySound( "melee.hitstone" );
				else if ( Material == StructureMaterial.Metal )
					PlaySound( "melee.hitmetal" );
			}
		}

		base.TakeDamage( info );
	}

	public override void OnContextAction( CollapsePlayer player, ContextAction action )
	{
		if ( Game.IsClient ) return;

		if ( action == StoneUpgradeAction && Material == StructureMaterial.Wood )
		{
			if ( player.HasItems<StoneItem>( StoneUpgradeCost ) )
			{
				Sound.FromWorld( To.Everyone, PlaceSoundName, Position );
				player.TakeItems<StoneItem>( StoneUpgradeCost );
				Material = StructureMaterial.Stone;
				Health = GetMaxHealth();
				UpdateMaterial();
			}
		}
		else if ( action == MetalUpgradeAction && Material < StructureMaterial.Metal )
		{
			Sound.FromWorld( To.Everyone, PlaceSoundName, Position );
			player.TakeItems<MetalFragments>( MetalUpgradeCost );
			Material = StructureMaterial.Metal;
			Health = GetMaxHealth();
			UpdateMaterial();
		}
	}

	protected virtual float GetMaxHealth()
	{
		if ( Material == StructureMaterial.Stone )
			return 500f;
		else if ( Material == StructureMaterial.Metal )
			return 1000f;
		else
			return 250f;
	}

	protected virtual void UpdateMaterial()
	{
		if ( Material == StructureMaterial.Stone )
			SetMaterialGroup( "stone" );
		else if ( Material == StructureMaterial.Metal )
			SetMaterialGroup( "metal" );
	}
}
