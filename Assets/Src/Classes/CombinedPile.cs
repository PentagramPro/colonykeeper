using System.Collections.Generic;
using UnityEngine;

public class CombinedPile : IListItem
{
	List<Pile> piles;


	public int Quantity{
		get{
			int res = 0;
			foreach(Pile p in piles)
				res += p.Quantity;
			return res;
		}
	}

	public string StringQuantity{
		get{
			return ((float)Quantity/100).ToString("0.00");
		}
	}

	public Pile FirstPile{
		get{
			return piles[0];
		}
	}

	public CombinedPile ()
	{
		piles = new List<Pile>();

	}

	public bool IsSame(Pile p)
	{
		if(piles.Count==0)
			return true;

		return piles[0].IsSameItem(p);
	}

	public bool IsSame(Item p)
	{
		if(piles.Count==0)
			return true;
		
		return piles[0].ItemType == p;
	}

	public void AddPile(Pile p)
	{
		if(IsSame(p) && !piles.Contains(p))
		{
			piles.Add(p);
		}
	}

	public void RemovePile(Pile p)
	{
		piles.Remove(p);
	}

	public bool IsEmpty()
	{
		return piles.Count==0;
	}

	#region IListItem implementation
	public string GetName ()
	{
		return piles[0].GetName();
	}
	#endregion
}


