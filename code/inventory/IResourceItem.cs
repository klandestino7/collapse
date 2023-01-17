namespace Facepunch.Collapse;

public interface IResourceItem
{
	public ItemResource Resource { get; }
	public void LoadResource( ItemResource resource );
}
