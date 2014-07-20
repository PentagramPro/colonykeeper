using UnityEngine;
using System.Collections.Generic;

public class ItemProps : IStorable
{
	UidContainer uidc;


	public Color color = new Color(1,1,1);
	public float conductivity = 1;
	public float durability = 1;
	public float hardness = 1;
	public float heatSustain = 1;

	public ItemProps()
	{
		uidc = new UidContainer(this);

	}

	public bool IsSameProperties(ItemProps p)
	{
		return p.color==color && 
			p.durability==durability && 
			p.conductivity==conductivity &&
			p.hardness == hardness &&
			p.heatSustain == heatSustain;
	}

	public ItemProps copy()
	{
		ItemProps res = new ItemProps();

		res.color = color;
		res.conductivity = conductivity;
		res.durability = durability;
		res.hardness = hardness;
		res.heatSustain = heatSustain;
		return res;
	}

	#region IStorable implementation
	public void SaveUid (WriterEx b)
	{
		uidc.SaveUid(b);
	}
	public void LoadUid (Manager m, ReaderEx r)
	{
		uidc.LoadUid(m,r);
	}
	public void Save (WriterEx b)
	{
		b.Write(color);
		b.Write((double)conductivity);
		b.Write((double)durability);
	}
	public void Load (Manager m, ReaderEx r)
	{
		color = r.ReadColor();
		conductivity = (float)r.ReadDouble();
		durability = (float)r.ReadDouble();
	}
	public int GetUID ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}


