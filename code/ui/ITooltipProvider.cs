using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace NxtStudio.Collapse.UI;

public interface ITooltipProvider : IValid
{
	public string Name { get; }
	public string Description { get; }
	public IReadOnlySet<string> Tags { get; }
	public bool IsVisible { get; }
	public Color Color { get; }
	public bool HasHovered { get;  }
	public void AddTooltipInfo( Panel container );
}
