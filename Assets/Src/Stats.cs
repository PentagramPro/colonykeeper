using UnityEngine;
using System.Collections.Generic;

public class Stats : MonoBehaviour {

	//public Dictionary<Item,int> Items = new Dictionary<Item, int>();
	public List<IListItem> ItemsList = new List<IListItem>();


	/*public int GetItemCount(Item item)
	{
		int res = 0;
		//Items.TryGetValue(item,out res);
		return res;
	}*/

	public void AddPile(Pile pile)
	{
		CombinedPile toAdd = null;
		foreach(IListItem i in ItemsList)
		{
			CombinedPile cp = i as CombinedPile;
			if(cp.IsSame(pile))
			{
				toAdd = cp;

				break;
			}
		}

		if(toAdd==null)
		{
			toAdd = new CombinedPile();
			ItemsList.Add(toAdd);
		}

		toAdd.AddPile(pile);
	}

	public void RemovePile(Pile pile)
	{
		CombinedPile toRemove=null;
		foreach(IListItem i in ItemsList)
		{
			CombinedPile cp = i as CombinedPile;
			if(cp.IsSame(pile))
			{
				cp.RemovePile(pile);
				if(cp.IsEmpty())
					toRemove = cp;
			}
		}

		if(toRemove!=null)
			ItemsList.Remove(toRemove);
	}



	public void GetItemsForIngredient(Ingredient ingredient,List<CombinedPile> result)
	{
		foreach(Item item in ingredient.Items)
		{
			foreach(IListItem t in ItemsList)
			{
				CombinedPile cp = t as CombinedPile;
				if(cp.IsSame(item))
					result.Add(cp);

			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
