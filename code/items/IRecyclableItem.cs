
using System.Collections.Generic;

namespace NxtStudio.Collapse;

public interface IRecyclableItem
{
	public Dictionary<string,int> RecycleOutput { get; }
	public bool IsRecyclable { get; }
	public string UniqueId { get; }
}