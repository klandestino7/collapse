using System.IO;

namespace NxtStudio.Collapse;

public static partial class BinaryWriterExtension
{
	public static void Write( this BinaryWriter self, PersistenceHandle item )
	{
		self.Write( item.Id );
	}
}
