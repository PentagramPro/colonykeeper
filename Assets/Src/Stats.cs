using UnityEngine;
using System.Collections.Generic;

public class Stats : MonoBehaviour {

	public Dictionary<Item,int> Items = new Dictionary<Item, int>();


	public int GetItemCount(Item item)
	{
		int res = 0;
		Items.TryGetValue(item,out res);
		return res;
	}

	public void ChangeItemCount(Item item, int delta)
	{
		if(Items.ContainsKey(item))
		{
			int count = Items[item];
			count+=delta;
			if(count>0)
				Items[item]=count;
			else
				Items.Remove(item);
		}
		else if(delta>0)
		{
			Items.Add(item,delta);
		}

	}

	public void GetItemsForIngredient(Ingredient ingredient,List<Item> result)
	{
		foreach(Item item in ingredient.Items)
		{
			if(Items[item]>=ingredient.Quantity)
				result.Add(item);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
