using System.Collections.Generic;
using UnityEngine;

public class SingleInventory : IInventory
{
	Pile pile;
	public float MaxQuantity = 5;



	public float Quantity{
		get{ return pile==null?0:pile.Quantity;}
	}

	#region IInventory implementation

	public override Pile Take (Item itemType, float quantity)
	{

		if(quantity<0)
			throw new UnityException("Negative values are not allowed!");

		float q = Mathf.Min(quantity,pile.Quantity);

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

	public override float Put (Item type, float quantity)
	{
		if(pile==null)
			pile = new Pile(type);
		else if(pile.ItemType!=type)
			return quantity;

		float free = MaxQuantity -pile.Quantity;

		pile.Quantity+=Mathf.Min(quantity,free);
		return Mathf.Max(0,quantity-free);
	}



	public override int CanTake(Item item)
	{
		if (pile == null)
			return 1;

		if (pile.ItemType == item && pile.Quantity<MaxQuantity)
			return 2;

		return 0;
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

	#endregion
}



