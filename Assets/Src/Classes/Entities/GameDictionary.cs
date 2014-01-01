using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("GameDictionary")]
public class GameDictionary  {

	[XmlArray("Blocks"),XmlArrayItem("Block")]
	public List<Block> Blocks = new List<Block>();

	[XmlArray("Items"),XmlArrayItem("Item")]
	public List<Item> Items = new List<Item>();
}
