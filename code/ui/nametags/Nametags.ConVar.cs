using Sandbox;

namespace NxtStudio.Collapse.UI;

public partial class Nametags
{
    [ConVar.Client( "fsk.nametag.self" )]
    public static bool ShowOwnNametag { get; set; }
}
