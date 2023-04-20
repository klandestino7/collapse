using System.IO;

namespace NxtStudio.Collapse;

public static partial class BinaryReaderExtension
{
	public static PersistenceHandle ReadPersistenceHandle( this BinaryReader buffer )
	{
		var id = buffer.ReadUInt64();
		return new PersistenceHandle( id );
	}
}
