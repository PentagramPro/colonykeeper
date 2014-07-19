using UnityEngine;
using System.Collections;

public class Pile : IStorable, IListItem {

	ItemProps Properties = new ItemProps();
	UidContainer uid;

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

	public Pile copy()
	{
		Pile res = new Pile(itemType,quantity);
		res.Properties = Properties.copy();
		return res;
	}

	public string StringQuantity{
		get{
			return ((float)quantity/100).ToString("0.00");
		}
	}

	public Pile(Item type)
	{
		itemType=type;
		uid = new UidContainer(this);
	}


	public Pile(Item type, int q)
	{
		itemType=type;
		Quantity=q;
		uid = new UidContainer(this);
	}

	public bool IsSameItem(Pile p)
	{
		return p.itemType == itemType && Properties.IsSameProperties(p.Properties);
	}


	#region IStorable implementation
	public void SaveUid(WriterEx b)
	{
		Properties.SaveUid(b);
	}
	
	public void LoadUid(Manager m, ReaderEx r)
	{
		Properties.LoadUid(m,r);
	}

	public void Save (WriterEx b)
	{
		b.Write(itemType.Name);
		b.Write(quantity);
		Properties.Save(b);
	}
	public void Load (Manager m, ReaderEx r)
	{
		itemType = m.GameD.Items[r.ReadString()];
		quantity = r.ReadInt32();
		Properties.Load(m,r);
	}

	public int GetUID()
	{
		return uid.UID;
	}
	#endregion

	#region IListItem implementation

	public string GetName ()
	{
		return ItemType.GetName();
	}

	#endregion
}
