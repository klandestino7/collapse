using Sandbox;

namespace NxtStudio.Collapse.UI;

public partial class LockScreen
{
    public static void OpenToLock( CollapsePlayer player, ICodeLockable entity )
    {
        OpenForClient( To.Single( player ), (Entity)entity, true );
    }

	public static void OpenToUnlock( CollapsePlayer player, ICodeLockable entity )
	{
		OpenForClient( To.Single( player ), (Entity)entity, false );
	}

	[ClientRpc]
    public static void OpenForClient( Entity entity, bool isLockMode )
    {
        if ( Game.LocalPawn is not CollapsePlayer ) return;
		if ( entity is not ICodeLockable lockable ) return;

        var lockScreen = Current;

		lockScreen.SetLockable( lockable, isLockMode );
		lockScreen.Open();

        Sound.FromScreen( "inventory.open" );
    }
}
