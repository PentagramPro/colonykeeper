using System;
using System.Collections.Generic;
using UnityEngine;

public class IInventory : BaseManagedController, IStorable
{
	public delegate void InventoryEvent();

	public event InventoryEvent ItemRemoved;

	public string ItemClass = "";
	public int MaxQuantity= 2000;
	Dictionary<Item, Pile> items = new Dictionary<Item, Pile>();
	int totalQuantity=0;

	public bool BlockInventory = false;
	public bool AllowSeveralTypes = true;

	// report in and out items to Stats module of Manager
	public bool ReportStatistic = true;

	public Pile[] Items
	{
		get
		{
			Pile[] p = new Pile[items.Count];
			int n =0;
			foreach(Pile i in items.Values)
			{
				p[n] = i;
				n++;
			}
			return p;
		}
	}

	public int Quantity{
		get{
			return totalQuantity;
		}
	}


	public void DropCrate()
	{
		if(Quantity==0)
			return;

		IInventory crate = M.CreateCrate(transform);
		foreach (Pile p in items.Values)
		{
			crate.Put(p);
		}
		items.Clear();
		totalQuantity = 0;
	}

	public Pile Take (Item itemType, int quantity)
	{
		if(quantity<0)
			throw new UnityException("Negative values are not allowed!");
		
		
		
		if ( items.Count==0 || !items.ContainsKey(itemType))
			return null;
		
		Pile pile = items[itemType];
		
		int q = Mathf.Min(quantity,pile.Quantity);		
		
		totalQuantity -= q;
		if(totalQuantity<0)
			throw new UnityException("Negative totalQuantity. Some bug in inventory implementation!");
		if (ItemRemoved != null)
			ItemRemoved();

		Pile res;

		if(pile.Quantity==q)
		{
			res=pile;
			items.Remove(itemType);
			if(ReportStatistic)
			{
				M.Stat.RemovePile(res);	
				//	M.Stat.ChangeItemCount(itemType,-q);
			}
		}
		else
		{
			pile.Quantity-=q;
			res = new Pile(pile.ItemType,q);
		}



		return res;
	}


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
	public virtual int Put (Item type, int quantity)
	{
		if(!type.IsOfClass(ItemClass))
			return quantity;
		
		int free = MaxQuantity -totalQuantity;
		if(free<=0)
			return quantity;

		if(!AllowSeveralTypes && items.Count>0 && !items.ContainsKey(type))
			return quantity;

		Pile pile = null;
		items.TryGetValue(type, out pile);
		if(pile==null)
		{
			pile = new Pile(type);
			items.Add(type,pile);
			if(ReportStatistic)
				M.Stat.AddPile(pile);
		}
		
		int toPut = Mathf.Min(quantity,free);
		totalQuantity+=toPut;
		pile.Quantity+=toPut;

		
		return Mathf.Max(0,quantity-free);
	}



	public void OnDestroy()
	{
		foreach(Pile p in items.Values)
		{
			if(ReportStatistic)
			{
				M.Stat.RemovePile(p);
				//M.Stat.ChangeItemCount(p.ItemType,-p.Quantity);
			}
		}
	}

	// returns:
	// 0 - cannot
	// 1 - can but does not prefer to take
	// 2 - prefers to take (i.e. single inventory that already contains that type of item)
	public int CanPut (Item item)
	{
		if(BlockInventory)
			return 0;

		if(!item.IsOfClass(ItemClass))
			return 0;

		if(AllowSeveralTypes)
		{
			if (totalQuantity<MaxQuantity)
				return 1;
		}
		else
		{
			if(items.Count==0)
				return 1;

			if(totalQuantity<MaxQuantity && items.ContainsKey(item))
				return 2;
		}
		
		return 0;
	}
	
	public bool CanTake (Item item)
	{
		if(BlockInventory)
			return false;

		if(items.Count==0)
			return false;
		
		return items.ContainsKey(item);
	}
	
	public Item[] GetItemTypes ()
	{
		Item[] res= new Item[items.Count];
		int x=0;
		foreach(Item i in items.Keys)
			res[x++] = i;
		
		return res;
		
	}

	public int GetItemQuantity (Item item)
	{
		Pile p;
		if(!items.TryGetValue(item,out p))
			return 0;
		return p.Quantity;
	}
	
	public bool IsFull ()
	{
		return totalQuantity>=MaxQuantity;
	}

	#region IStorable implementation
	public void Save (WriterEx b)
	{
		b.Write(items.Values.Count);
		foreach (Pile p in items.Values)
		{
			p.Save(b);
		}
	}
	
	public void Load (Manager m, ReaderEx r)
	{
		items.Clear();
		totalQuantity = 0;
		int count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			Pile p = new Pile(null);
			p.Load(m,r);
			items.Add(p.ItemType,p);
			totalQuantity+=p.Quantity;
			m.Stat.AddPile(p);
		}
	}
	#endregion
}


