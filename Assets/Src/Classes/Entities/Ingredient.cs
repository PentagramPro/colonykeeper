
using System.Collections.Generic;

public class Ingredient
{
	public int Quantity;

	// each of the items from the list can be used as an ingredient
	public List<Item> Items = new List<Item>();

	public string ClassName;

	public bool Contains(Item item)
	{
		return Items.Contains(item);
	}

}

