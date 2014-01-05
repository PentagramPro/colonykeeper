using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("GameDictionary")]
public class GameDictionary  {

	[XmlArray("Blocks"),XmlArrayItem("Block")]
	public List<Block> Blocks = new List<Block>();

	[XmlArray("Items"),XmlArrayItem("Item")]
	public List<Item> Items = new List<Item>();

	[XmlIgnore]
	public List<Block> CellBlocks = new List<Block>();

	[XmlIgnore]
	public List<Block> ObjectBlocks = new List<Block>();

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
		CellBlocks.Clear();
		ObjectBlocks.Clear();

		foreach(Block b in Blocks)
		{
			if(!string.IsNullOrEmpty(b.MaterialName))
			{
				CellBlocks.Add(b);
			}
			else if(!string.IsNullOrEmpty(b.PrefabName))
			{
				ObjectBlocks.Add(b);
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
		catch (FileNotFoundException e)
		{
			res =  new GameDictionary();
		}

		res.Sort();

		return res;
	}
}

