using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Block  {
	public class LootRec
	{
		[XmlAttribute("p")]
		public float p=1.0f;

		[XmlAttribute("quantity")]
		public float quantity = 1.0f;

		[XmlText]
		public string ItemName;

		[XmlIgnore]
		public Item item;
	}

	[XmlArray("Loot"),XmlArrayItem("LootRec")]
	public List<LootRec> Loot = new List<LootRec>();

	[XmlAttribute("PrefabName")]
	public string PrefabName;

	[XmlAttribute("MaterialName")]
	public string MaterialName;
}
