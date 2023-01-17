namespace Facepunch.Collapse.UI;

public interface IDialog
{
	bool IsOpen { get; }
	void Open();
	void Close();
}
