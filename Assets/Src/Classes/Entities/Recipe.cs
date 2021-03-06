using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : IListItem
{



	[XmlAttribute("Device")]
	public string Device;

	[XmlAttribute("Name")]
	public string Name;

	[XmlArray("Ingredients"),XmlArrayItem("Ingredient")]
	public List<Ingredient> Ingredients = new List<Ingredient>();

	[XmlArray("Results"),XmlArrayItem("Result")]
	public List<PileXML> Results = new List<PileXML>();

	//[XmlIgnore]
	public List<Ingredient> IngredientsLinks
	{
		get
		{
			return Ingredients;
		}
	}

	[XmlIgnore]
	public List<Pile> ResultsLinks = new List<Pile>();


	// used only for vehicle recipes
	// points to result of this recipe
	[XmlIgnore]
	public VehicleProt vehicle;

	[XmlIgnore]
	public bool IsOneCombination
	{
		get{
			foreach(Ingredient i in IngredientsLinks)
				if(i.Items.Count>1)
					return false;
			return true;
		}
	}
	public override string ToString ()
	{
		return Name;
	}

	public int GetIngredient(Item itemType)
	{
		foreach (Ingredient i in IngredientsLinks)
		{
			if(i.Contains(itemType))
				return i.Quantity;
		}
			
		return 0;
	}

	public void Sort(GameDictionary g)
	{

		foreach(Ingredient ing in Ingredients)
		{

			//Ingredient ing = new Ingredient();
			//ing.Quantity = pxml.Quantity;

			if(ing.Name.EndsWith(".*"))
			{
				string cls = ing.Name.Substring(0,ing.Name.Length-2);
				ing.ClassName = cls;
				foreach(Item i in g.Items.Values)
				{
					if(i.IsOfClass(cls))
						ing.Items.Add(i);
				}
			}
			else
			{
				ing.ClassName = "";
				if(!g.Items.ContainsKey(ing.Name))
					throw new UnityException("Item with name "+ing.Name+" was not found while building ingredients for recipe "+Name);
				ing.Items.Add(g.Items[ing.Name]);
			}
			/*
			if(ing.Items.Count>0)
				IngredientsLinks.Add(ing);
			else
				Debug.LogWarning("No items found for recipe "+Name+", item name="+ing.Name);*/
		}
		
		foreach(PileXML pxml in Results)
		{
			if(g.Items.ContainsKey(pxml.Name))
				ResultsLinks.Add(new Pile(g.Items[pxml.Name],pxml.Quantity));
			else 
				throw new UnityException("Item with name "+pxml.Name+" wasn`t found in dictionary  while sorting recipe results. Recipe name is "+Name);
		}
	}

	#region IListItem implementation
	public string GetName ()
	{
		return Name;
	}
	#endregion
}


