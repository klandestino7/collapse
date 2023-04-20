using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace NxtStudio.Collapse.UI;

[StyleSheet( "/ui/ToolboxMenu.scss" )]
public partial class ToolboxMenu : RadialMenu
{
	public static ToolboxMenu Current { get; private set; }

	public override InputButton Button => InputButton.SecondaryAttack;

	public ToolboxMenu()
	{
		Current = this;
	}

	public override void Populate()
	{
		var descriptions = new List<TypeDescription>
		{
			TypeLibrary.GetType<Foundation>(),
			TypeLibrary.GetType<Doorway>(),
			TypeLibrary.GetType<Wall>()
		};

		var player = CollapsePlayer.Me;

		foreach ( var type in descriptions )
		{
			var name = type.Name;
			var title = type.Title;
			var description = type.Description;
			var costs = Structure.GetCostsFor( type );
			var item = AddItem( title, description, type.Icon, () => Select( name ) );

			if ( player.IsValid() )
			{
				var canAfford = true;

				foreach ( var kv in costs )
				{
					if ( !player.HasItems( kv.Key, kv.Value ) )
					{
						canAfford = false;
						break;
					}
				}

				item.Subtitle = string.Join( ", ", costs.Select( kv => GetCostString( kv ) ) );
				item.RootClass = canAfford ? string.Empty : "cannot-afford";
			}
		}

		base.Populate();
	}

	protected override bool ShouldOpen()
	{
		if ( !CollapsePlayer.Me.IsValid() )
			return false;

		return (CollapsePlayer.Me.GetActiveHotbarItem() is ToolboxItem);
	}

	private string GetCostString( KeyValuePair<string,int> pair )
	{
		var definition = InventorySystem.GetDefinition( pair.Key );
		return $"{definition.Name} x {pair.Value}";
	}

	private void Select( string typeName )
	{
		var type = TypeLibrary.GetType( typeName );
		CollapsePlayer.Me.SetStructureType( type );
	}
}
