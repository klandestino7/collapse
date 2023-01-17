using Sandbox;
using System.Collections.Generic;

namespace NxtStudio.Collapse;

public partial class CollapsePlayer
{
	protected List<ArmorEntity> ArmorEntities { get; set; } = new();

	public ArmorEntity AttachArmor( string modelName, ArmorItem item )
	{
		var entity = new ArmorEntity();
		entity.SetModel( modelName );
		AttachArmor( entity, item );
		return entity;
	}

	public void AttachArmor( ArmorEntity clothing, ArmorItem item )
	{
		clothing.SetParent( this, true );
		clothing.EnableShadowInFirstPerson = true;
		clothing.EnableHideInFirstPerson = true;
		clothing.Item = item;

		ArmorEntities.Add( clothing );
	}

	public void RemoveArmor()
	{
		ArmorEntities.ForEach( ( entity ) =>
		{
			entity.Delete();
		} );

		ArmorEntities.Clear();
	}
}
