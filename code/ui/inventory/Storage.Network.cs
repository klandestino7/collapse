using Sandbox;

namespace NxtStudio.Collapse.UI;

public partial class Storage
{
    public static void Open( CollapsePlayer player, string name, Entity entity, InventoryContainer container )
    {
        OpenForClient( To.Single(player), name, entity, container.Serialize() );

		var viewer = player.Client.Components.Get<InventoryViewer>();
		viewer?.AddContainer( container );
    }

    [ClientRpc]
    public static void OpenForClient( string name, Entity entity, byte[] data )
    {
        if ( Game.LocalPawn is not CollapsePlayer ) return;

        var container = InventoryContainer.Deserialize( data );
        var storage = Current;

		if ( container.IsEmpty ) return;

        storage.SetName( name );
        storage.SetEntity( entity );
        storage.SetContainer( container );
        storage.Open();

        Sound.FromScreen( "inventory.open" );
    }
}
