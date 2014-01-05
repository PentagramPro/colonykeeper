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

	public GameObject Instantiate()
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for Block with empty PrefabName");

		GameObject obj = Resources.Load<GameObject>("Prefabs/Blocks/"+PrefabName);

		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);

		BlockController bc = obj.GetComponent<BlockController>();

		if(bc==null)
			throw new UnityException("No BlockController attached to prefab "+PrefabName);

		bc.BlockProt = this;

		return obj;
	}

	[XmlArray("Loot"),XmlArrayItem("LootRec")]
	public List<LootRec> Loot = new List<LootRec>();

	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("PrefabName")]
	public string PrefabName;

	[XmlAttribute("MaterialName")]
	public string MaterialName;
}
