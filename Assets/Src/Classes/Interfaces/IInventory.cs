using System;
using UnityEngine;

public abstract class IInventory : MonoBehaviour
{
	public abstract Pile Take(Item itemType, float quantity);

	// returns, how many items left (couldn`t be moved to
	// this inventory)
	public float Put(Pile item)
	{
		return Put (item.ItemType,item.Quantity);
	}

	// returns, how many items left (couldn`t be moved to
	// this inventory)
	public abstract float Put(Item type, float quantity);


	public bool Put(IInventory source, float amount, Item type)
	{
		Pile taken = source.Take(type,amount);
		if(taken==null)
			return false;
		float put = Put(taken);
		
		if(put>0)
		{
			taken.Quantity=put;
			source.Put(taken);
		}

		return true;
	}

	// returns:
	// 0 - cannot
	// 1 - can but does not prefer to take
	// 2 - prefers to take (i.e. single inventory that already contains that type of item)
	public abstract int CanTake(Item item);
	public abstract Item[] GetItemTypes();

	public abstract bool IsFull();
}


