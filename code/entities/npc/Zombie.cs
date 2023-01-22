using Editor;
using Sandbox;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NxtStudio.Collapse.NPC;

[HammerEntity]
[Title( "Zombie" )]
[Model( Model = "models/citizen/citizen.vmdl" )]
public partial class Zombie : ZombieBase
{
	
	public Zombie()
	{
		
	}

	public override void Spawn()
	{

		base.Spawn();
	}
}