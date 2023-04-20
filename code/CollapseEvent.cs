using Sandbox;

namespace NxtStudio.Collapse;

public static class CollapseEvent
{
	public class NavBlockerAdded : EventAttribute
	{
		public NavBlockerAdded() : base( "fsk.navblocker.added" )
		{

		}
	}

	public class NavBlockerRemoved : EventAttribute
	{
		public NavBlockerRemoved() : base( "fsk.navblocker.removed" )
		{

		}
	}
}
