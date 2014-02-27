using UnityEngine;
using System.Collections.Generic;

public class ItemProps : IStorable
{
	UidContainer uidc;


	public Color color = new Color(1,1,1);
	public float speed = 1;
	public float durability = 1;


	public ItemProps()
	{
		uidc = new UidContainer(this);

	}

	public bool IsSameProperties(ItemProps p)
	{
		return p.color==color && p.durability==durability && p.speed==speed;
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
		b.Write((double)speed);
		b.Write((double)durability);
	}
	public void Load (Manager m, ReaderEx r)
	{
		color = r.ReadColor();
		speed = (float)r.ReadDouble();
		durability = (float)r.ReadDouble();
	}
	public int GetUID ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}


