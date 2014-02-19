using UnityEngine;
using System.Collections;

public class Pile : IStorable {

	Item itemType;
	public Item ItemType
	{
		get{return itemType;}
	}

	private int quantity=0;
	public int Quantity{
		get{
			return quantity;
		}
		set{
			quantity = value;
		}
	}

	public Pile(Item type)
	{
		itemType=type;
	}


	public Pile(Item type, int q)
	{
		itemType=type;
		Quantity=q;
	}

	#region IStorable implementation
	public void SaveUid(WriterEx b)
	{
	
	}
	
	public void LoadUid(Manager m, ReaderEx r)
	{
	
	}

	public void Save (WriterEx b)
	{
		b.Write(itemType.Name);
		b.Write(quantity);
	}
	public void Load (Manager m, ReaderEx r)
	{
		itemType = m.GameD.Items[r.ReadString()];
		quantity = r.ReadInt32();
	}
	#endregion
}
