using System;
using System.Collections.Generic;
using UnityEngine;

public class IInventory : BaseManagedController, IStorable
{
	public delegate void InventoryEvent();

	public event InventoryEvent ItemRemoved;

	public string ItemClass = "";
	public int MaxQuantity= 2000;
	Dictionary<Item, List<Pile>> items = new Dictionary<Item, List<Pile>>();
	int totalQuantity=0;

	public bool BlockInventory = false;
	public bool AllowSeveralTypes = true;

	// report in and out items to Stats module of Manager
	public bool ReportStatistic = true;

	void EnumeratePiles(Action<Pile> callback)
	{
		foreach(List<Pile> lp in items.Values)
		{
			foreach(Pile p in lp)
			{
				callback(p);
			}
		}
	}

	public Pile[] Items
	{
		get
		{
			List<Pile> l = new List<Pile>();
			EnumeratePiles((Pile p)=>{
				l.Add(p);
			});
			Pile[] res = new Pile[l.Count];

			int n =0;
			foreach(Pile i in l)
			{
				res[n] = i;
				n++;
			}
			return res;
		}
	}

	public int Quantity{
		get{
			return totalQuantity;
		}
	}

	public Pile FirstPile
	{
		get
		{
			foreach(List<Pile> p in items.Values)
				return p[0];
			return null;
		}
	}


	public void DropCrate()
	{
		if(Quantity==0)
			return;

		IInventory crate = M.CreateCrate(transform);
		EnumeratePiles((Pile p)=>{
			crate.Put(p);
		});

		items.Clear();
		totalQuantity = 0;
	}
	/*
	public Pile Take (Pile prototype, int quantity)
	{
		return Take (prototype,quantity,false);
	}*/
	public Pile Take (PileRequest prototype)
	{
		if(prototype.Quantity<0)
			throw new UnityException("Negative values are not allowed!");
		
		
		
		if ( items.Count==0 || !items.ContainsKey(prototype.ItemType))
			return null;


		List<Pile> storedPiles = items[prototype.ItemType];
		Pile targetPile = null;
		if(prototype.HasProperties)
		{
			foreach(Pile p in storedPiles)
			{
				if(p.IsSameItem(prototype))
				{
					targetPile = p;
					break;
				}
				
			}

		}
		else
		{
			if(storedPiles.Count>0)
				targetPile = storedPiles[0];
		}
		if(targetPile==null)
			return null;

		int q = Mathf.Min(prototype.Quantity,targetPile.Quantity);		
		
		totalQuantity -= q;
		if(totalQuantity<0)
			throw new UnityException("Negative totalQuantity. Some bug in inventory implementation!");
		if (ItemRemoved != null)
			ItemRemoved();

		Pile res;

		if(targetPile.Quantity==q)
		{
			res=targetPile;
			storedPiles.Remove(targetPile);
			if(storedPiles.Count==0)
				items.Remove(targetPile.ItemType);

			if(ReportStatistic)
			{
				M.Stat.RemovePile(res);	
				//	M.Stat.ChangeItemCount(itemType,-q);
			}
		}
		else
		{
			res = targetPile.copy();
			targetPile.Quantity-=q;
			res.Quantity = q;

		}



		return res;
	}


	public bool Put(IInventory source, Pile prototype, int quantity)
	{
		Pile taken = source.Take(new PileRequest(prototype,quantity));
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

		//check if item has proper class
		if(!item.ItemType.IsOfClass(ItemClass))
			return item.Quantity;

		// checking free space
		int free = MaxQuantity -totalQuantity;
		if(free<=0)
			return item.Quantity;


		// if this inventory can hold only signe type of item, we check it
		if(!AllowSeveralTypes && items.Count>0 && !items.ContainsKey(item.ItemType))
			return item.Quantity;

		List<Pile> pileList;

		int toPut = Mathf.Min(item.Quantity,free);
		int left = Mathf.Max(0,item.Quantity-free);
		item.Quantity = toPut;
		totalQuantity += toPut;

		if(items.TryGetValue(item.ItemType,out pileList))
		{
			Pile targetPile = null;
			foreach(Pile p in pileList)
			{
				if(p.IsSameItem(item))
				{
					targetPile = p;
					break;
				}
			}
			if(targetPile==null)
			{
				pileList.Add(item.copy());
			}
			else
			{
				targetPile.Quantity+=item.Quantity;
			}
		}
		else
		{
			pileList = new List<Pile>();
			items.Add(item.ItemType,pileList);
			pileList.Add(item.copy());
		}





		return left;
	}

//	// returns, how many items left (couldn`t be moved to
//	// this inventory)
//	public virtual int Put (Item type, int quantity)
//	{
//		if(!type.IsOfClass(ItemClass))
//			return quantity;
//		
//		int free = MaxQuantity -totalQuantity;
//		if(free<=0)
//			return quantity;
//
//		if(!AllowSeveralTypes && items.Count>0 && !items.ContainsKey(type))
//			return quantity;
//
//		Pile pile = null;
//		items.TryGetValue(type, out pile);
//		if(pile==null)
//		{
//			pile = new Pile(type);
//			items.Add(type,pile);
//			if(ReportStatistic)
//				M.Stat.AddPile(pile);
//		}
//		
//		int toPut = Mathf.Min(quantity,free);
//		totalQuantity+=toPut;
//		pile.Quantity+=toPut;
//
//		
//		return Mathf.Max(0,quantity-free);
//	}
//


	public void OnDestroy()
	{
		EnumeratePiles((Pile p)=>{
			M.Stat.RemovePile(p);
		});
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



	public bool CanTake(PileRequest pile, bool checkQuantity)
	{
		if(BlockInventory)
			return false;

		if(!items.ContainsKey(pile.ItemType))
			return false;

		List<Pile> list = items[pile.ItemType];
		foreach(Pile p in list)
		{
			if(p.IsSameItem(pile) && (!checkQuantity || pile.Quantity<=p.Quantity))
				return true;
		}
		return false;
	}
	/*public bool CanTake (Item item)
	{
		if(BlockInventory)
			return false;

		if(items.Count==0)
			return false;
		
		return items.ContainsKey(item);
	}*/
	
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
		List<Pile> l;
		if(!items.TryGetValue(item,out l))
			return 0;
		int res = 0;
		foreach(Pile p in l)
			res+=p.Quantity;
		return res;
	}
	
	public bool IsFull ()
	{
		return totalQuantity>=MaxQuantity;
	}

	#region IStorable implementation
	public void Save (WriterEx b)
	{
//		b.Write(items.Values.Count);
//		foreach (Pile p in items.Values)
//		{
//			p.Save(b);
//		}
	}
	
	public void Load (Manager m, ReaderEx r)
	{
//		items.Clear();
//		totalQuantity = 0;
//		int count = r.ReadInt32();
//		for(int i=0;i<count;i++)
//		{
//			Pile p = new Pile(null);
//			p.Load(m,r);
//			items.Add(p.ItemType,p);
//			totalQuantity+=p.Quantity;
//			m.Stat.AddPile(p);
//		}
	}
	#endregion
}


