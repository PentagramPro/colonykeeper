using System.Collections.Generic;
using UnityEngine;

public class SingleInventory : IInventory
{
	Pile pile;
	public SingleInventory ()
	{

	}

	#region IInventory implementation

	public Pile Take (float quantity)
	{

		if(quantity<0)
			throw new UnityException("Negative values are not allowed!");

		float q = Mathf.Min(quantity,pile.Quantity);

		if(pile.Quantity==q)
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

	public bool Put (Item type, float quantity)
	{
		if(pile==null)
			pile = new Pile(type);
		else if(pile.ItemType!=type)
			return false;
		
		pile.Quantity+=quantity;
		return true;
	}

	public bool Put (Pile item)
	{
		return Put (item.ItemType,item.Quantity);

	}

	#endregion
}


