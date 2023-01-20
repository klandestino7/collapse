using Sandbox;
using System.Collections.Generic;
using System.IO;

namespace NxtStudio.Collapse.NPC;

public partial class NPC : AnimatedEntity
{
	/// <summary>
	/// The display name of the NPC.
	/// </summary>
	[Net, Property] public string DisplayName { get; set; } = "NPC";

	/// <summary>
	/// Whether or not the NPC randomly wanders around the map.
	/// </summary>
	[Property] public bool DoesWander { get; set; } = false;

    // DamageInfo LastDamage;

	// public override void Spawn()
	// {
    //     base.Spawn();
    //     Tags.Add( "npc" );
	// }

}
