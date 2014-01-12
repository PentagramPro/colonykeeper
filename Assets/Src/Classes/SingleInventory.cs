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

	public bool Put (Pile item)
	{
		if(pile==null)
			pile = new Pile(item.ItemType);
		else if(pile.ItemType!=item.ItemType)
			return false;

		pile.Quantity+=item.Quantity;
		return true;

	}

	#endregion
}


