﻿using UnityEngine;
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

	[XmlArray("Buildings"),XmlArrayItem("Building")]
	public List<Building> Buildings = new List<Building>();



	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(GameDictionary));
	
		using(var stream = new FileStream(path, FileMode.Create))			
		{
			serializer.Serialize(stream, this);	
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

		res.Blocks[0].Breakable=false;
		return res;
	}
}

