
namespace Facepunch.Collapse;

public interface ILootTableItem
{
	public int AmountToSpawn { get; }
	public float SpawnChance { get; }
	public bool IsLootable { get; }
	public string UniqueId { get; }
}
