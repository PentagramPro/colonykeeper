
using System.Collections.Generic;

public class Ingredient
{
	public int Quantity;

	// each of the items from the list can be used as an ingredient
	public List<Item> Items = new List<Item>();

	// list of property names. These properties will be taken from this ingredient during craft
	public List<string> Properties = new List<string>();

	public string ClassName;

	public bool Contains(Item item)
	{
		return Items.Contains(item);
	}

}

