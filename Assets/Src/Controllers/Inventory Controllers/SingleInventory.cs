using System.Collections.Generic;
using UnityEngine;
using System;

public class SingleInventory : IInventory
{
	Pile pile;
	public int MaxQuantity = 2000;



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
			return res;
		}
		else
		{
			pile.Quantity-=q;
			return new Pile(pile.ItemType,q);
		}
	}

	public override int Put (Item type, int quantity)
	{
		if(pile==null)
			pile = new Pile(type);
		else if(pile.ItemType!=type)
			return quantity;

		int free = MaxQuantity -pile.Quantity;

		pile.Quantity+=Mathf.Min(quantity,free);
		return Mathf.Max(0,quantity-free);
	}



	public override int CanPut(Item item)
	{
		if (pile == null)
			return 1;

		if (pile.ItemType == item && pile.Quantity<MaxQuantity)
			return 2;

		return 0;
	}

	public override bool CanTake (Item item)
	{
		return pile==null || (pile.Quantity<MaxQuantity && pile.ItemType==item);
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
}



