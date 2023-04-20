using Sandbox;

namespace NxtStudio.Collapse.UI;

public partial class Recycling
{
    public static void Open( CollapsePlayer player, string name, Recycler entity )
    {
		var processor = entity.Processor;

        OpenForClient( To.Single( player ), name, entity, processor.Input.Serialize(), processor.Output.Serialize() );

		var viewer = player.Client.Components.Get<InventoryViewer>();
		viewer?.AddContainer( processor.Input );
		viewer?.AddContainer( processor.Output );
    }

    [ClientRpc]
    public static void OpenForClient( string name, Recycler entity, byte[] input, byte[] output )
    {
        if ( Game.LocalPawn is not CollapsePlayer ) return;

        var recycling = Current;

		InventoryContainer.Deserialize( input );
		InventoryContainer.Deserialize( output );

		recycling.SetName( name );
		recycling.SetRecycler( entity );
		recycling.Open();

        Sound.FromScreen( "inventory.open" );
    }
}
