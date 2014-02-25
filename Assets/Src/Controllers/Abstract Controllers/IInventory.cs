using System;
using UnityEngine;

public abstract class IInventory : BaseManagedController, IStorable
{
	public delegate void InventoryEvent();

	public abstract Pile Take(Item itemType, int quantity);

	// returns, how many items left (couldn`t be moved to
	// this inventory)
	public int Put(Pile item)
	{
		if(item==null)
			return 0;
		return Put (item.ItemType,item.Quantity);
	}

	// returns, how many items left (couldn`t be moved to
	// this inventory)
	public abstract int Put(Item type, int quantity);



	public bool Put(IInventory source, int amount, Item type)
	{
		Pile taken = source.Take(type,amount);
		if(taken==null)
			return false;
		int put = Put(taken);
		
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
	public abstract int CanPut(Item item);

	// true if you can take this item from this inventory
	public abstract bool CanTake(Item item);

	public abstract Item[] GetItemTypes();

	public abstract int GetItemQuantity(Item item);

	public abstract bool IsFull();


	#region IStorable implementation
	public abstract void Save (WriterEx b);
	public abstract void Load (Manager m, ReaderEx r);
	#endregion
}


