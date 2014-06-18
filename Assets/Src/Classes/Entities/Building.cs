using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Building 
{
	public GameObject Instantiate()
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for Block with empty PrefabName");
		
		GameObject obj = Resources.Load<GameObject>("Prefabs/Blocks/"+PrefabName);

		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);

		obj = (GameObject)GameObject.Instantiate(obj);
		BuildingController bc = obj.GetComponent<BuildingController>();
		if (bc != null)
		{
			bc.Name = Name;
		}
		return obj;
	}


	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("PrefabName")]
	public string PrefabName;

	[XmlArray("Ingredients"),XmlArrayItem("Ingredient")]
	public List<PileXML> Ingredients = new List<PileXML>();

	[XmlAttribute("Hide")]
	public bool Hide = false;
	[XmlIgnore]
	public Recipe recipe;
}


