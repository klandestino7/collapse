using Sandbox;

namespace NxtStudio.Collapse.UI;

public partial class Thoughts
{
	private static TimeSince LastThoughtTime { get; set; }
	private static string LastThoughtId { get; set; }

	[ClientRpc]
	public static void Show( string id, string thought, bool ignoreCooldown = false )
	{
		if ( !ignoreCooldown && LastThoughtId == id && LastThoughtTime < 3f )
		{
			return;
		}

		var entry = Instance?.AddEntry( thought );
		entry?.AddClass( id );

		Sound.FromScreen( "thought" );

		LastThoughtTime = 0f;
		LastThoughtId = id;
	}

	[ClientRpc]
	public static void Show( string thought, bool ignoreCooldown = false )
	{
		if ( !ignoreCooldown && LastThoughtId == thought && LastThoughtTime < 3f )
		{
			return;
		}

		Instance?.AddEntry( thought );
		Sound.FromScreen( "thought" );

		LastThoughtId = thought;
		LastThoughtTime = 0f;
	}
}
