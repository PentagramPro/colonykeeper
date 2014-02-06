using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("GameDictionary")]
public class GameDictionary  {


	[XmlIgnore]
	public Dictionary<string, Item> Items = new Dictionary<string, Item>();

	[XmlArray("Blocks"),XmlArrayItem("Block")]
	public List<Block> Blocks = new List<Block>();

	[XmlArray("Items"),XmlArrayItem("Item")]
	public List<Item> ItemsList = new List<Item>();

	[XmlArray("StartItems"),XmlArrayItem("Item")]
	public List<PileXML> StartItemsList = new List<PileXML>();

	[XmlArray("Buildings"),XmlArrayItem("Building")]
	public List<Building> Buildings = new List<Building>();

	[XmlArray("Recipes"),XmlArrayItem("Recipe")]
	public List<Recipe> Recipes = new List<Recipe>();

	[XmlIgnore]
	public Dictionary<string, List<Recipe>> RecipesByDevice = new Dictionary<string, List<Recipe>>();


	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(GameDictionary));
	
		using(var stream = new FileStream(path, FileMode.Create))			
		{
			serializer.Serialize(stream, this);	
		}
		
	}

	void Sort()
	{
		foreach(Item i in ItemsList)
		{
			Items.Add(i.Name,i);

		}
		ItemsList.Clear();

		foreach(Block b in Blocks)
		{
			if(b.Contains!=null)
				b.ContainsItem = Items[b.Contains];
		}

		RecipesByDevice.Clear();
		foreach (Recipe r in Recipes)
		{
			List<Recipe> list = null;
			RecipesByDevice.TryGetValue(r.Device,out list);

			if(list==null)
			{
				list = new List<Recipe>();
				RecipesByDevice.Add(r.Device,list);
			}
			list.Add(r);

			foreach(PileXML pxml in r.Ingredients)
			{
				r.IngredientsLinks.Add(new Pile(Items[pxml.Name],pxml.Quantity));
			}

			foreach(PileXML pxml in r.Results)
			{
				r.ResultsLinks.Add(new Pile(Items[pxml.Name],pxml.Quantity));
			}
		}

		foreach(Building b in Buildings)
		{
			foreach(PileXML pxml in b.Ingredients)
			{
				b.recipe = new Recipe();
				b.recipe.IngredientsLinks.Add(new Pile(Items[pxml.Name],pxml.Quantity));
			}
		}
	}

	public static GameDictionary Load(string path)
	{
		var serializer = new XmlSerializer(typeof(GameDictionary));
		GameDictionary res;
		try
		{
			using(var stream = new FileStream(path, FileMode.Open))	
			{
				res =  serializer.Deserialize(stream) as GameDictionary;	
			}
		}
		catch (FileNotFoundException)
		{
			res =  new GameDictionary();
		}
		res.Sort();
		res.Blocks[0].Breakable=false;
		return res;
	}
}

