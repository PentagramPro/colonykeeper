using System.Collections.Generic;
using UnityEngine;
using System;

public class SingleInventory : IInventory, IStorable
{
	Pile pile;
	public int MaxQuantity = 2000;

	public String ItemClass = "";

	public int Quantity{
		get{ return pile==null?0:pile.Quantity;}
	}


	#region IInventory implementation

	public override Pile Take (Item itemType, int quantity)
	{

		if(quantity<0)
			throw new UnityException("Negative values are not allowed!");

		int q = Math.Min(quantity,pile.Quantity);

		if (pile == null || pile.ItemType!=itemType)
			return null;


		else if(pile.Quantity==q)
		{
			Pile res=pile;
			pile=null;
			M.Stat.ChangeItemCount(itemType,-q);
			return res;
		}
		else
		{
			pile.Quantity-=q;
			M.Stat.ChangeItemCount(itemType,-q);
			return new Pile(pile.ItemType,q);
		}

	}

	public override int Put (Item type, int quantity)
	{
		if(!type.IsOfClass(ItemClass))
			return quantity;
		else if(pile==null)
			pile = new Pile(type);
		else if(pile.ItemType!=type)
			return quantity;

		int free = MaxQuantity -pile.Quantity;
		int delta = Mathf.Min(quantity,free);
		pile.Quantity+=delta;
		M.Stat.ChangeItemCount(type,delta);

		return Mathf.Max(0,quantity-free);
	}



	public override int CanPut(Item item)
	{
		if(!item.IsOfClass(ItemClass))
			return 0;

		if (pile == null)
			return 1;

		if (pile.ItemType == item && pile.Quantity<MaxQuantity)
			return 2;

		return 0;
	}

	public override bool CanTake (Item item)
	{
		if(pile==null)
			return false;

		return (pile.Quantity>0 && pile.ItemType==item);
	}

	public override Item[] GetItemTypes ()
	{
		Item[] res = new Item[pile==null?0:1];
		if(res.Length>0)
			res [0] = pile.ItemType;
		return res;
	}

	public override bool IsFull()
	{
		return pile!=null && pile.Quantity>=MaxQuantity ;
	}

	public override int GetItemQuantity (Item item)
	{
		if(pile==null)
			return 0;

		if(item==pile.ItemType)
			return Quantity;

		return 0;
	}

	#endregion

	#region IStorable implementation

	public override void Save(WriterEx b)
	{
		b.Write(Quantity);
		if(pile!=null)
			pile.Save(b);

	}

	public override void Load(Manager m, ReaderEx r)
	{
		if(r.ReadInt32()>0)
		{
			pile = new Pile(null);
			pile.Load(m,r);
		}
		else
			pile = null;
	}

	#endregion
}



