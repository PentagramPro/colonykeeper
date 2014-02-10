using UnityEngine;
using System.Collections.Generic;

public class MultiInventory : IInventory {

	public string ItemClass = "";
	public int MaxQuantity= 2000;
	Dictionary<Item, Pile> items = new Dictionary<Item, Pile>();
	int totalQuantity=0;

	public int Quantity{
		get{
			return totalQuantity;
		}
	}

	#region implemented abstract members of IInventory
	public override Pile Take (Item itemType, int quantity)
	{
		if(quantity<0)
			throw new UnityException("Negative values are not allowed!");
		

		
		if ( items.Count==0 || !items.ContainsKey(itemType))
			return null;

		Pile pile = items[itemType];

		int q = Mathf.Min(quantity,pile.Quantity);		

		totalQuantity -=q;
		if(totalQuantity<0)
			throw new UnityException("Negative totalQuantity. Some bug in inventory implementation!");

		if(pile.Quantity==q)
		{
			Pile res=pile;
			items.Remove(itemType);
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
		if(!type.IsOfClass(ItemClass))
			return quantity;

		int free = MaxQuantity -totalQuantity;
		if(free<=0)
			return quantity;

		Pile pile = null;
		items.TryGetValue(type, out pile);
		if(pile==null)
		{
			pile = new Pile(type);
			items.Add(type,pile);
		}

		int toPut = Mathf.Min(quantity,free);
		totalQuantity+=toPut;
		pile.Quantity+=toPut;

		return Mathf.Max(0,quantity-free);
	}

	public override int CanPut (Item item)
	{
		if(!item.IsOfClass(ItemClass))
			return 0;
		
		if (totalQuantity<MaxQuantity)
			return 1;
		
		return 0;
	}

	public override bool CanTake (Item item)
	{
		if(items.Count==0)
			return false;

		return items.ContainsKey(item);
	}

	public override Item[] GetItemTypes ()
	{
		Item[] res= new Item[items.Count];
		int x=0;
		foreach(Item i in items.Keys)
			res[x++] = i;

		return res;

	}

	public override int GetItemQuantity (Item item)
	{
		Pile p;
		if(!items.TryGetValue(item,out p))
			return 0;
		return p.Quantity;
	}

	public override bool IsFull ()
	{
		return totalQuantity>=MaxQuantity;
	}
	#endregion
}
