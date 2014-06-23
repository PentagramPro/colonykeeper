using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Building : ObjectPrototype
{
	public override GameObject Instantiate()
	{

		GameObject obj = base.Instantiate("Buildings",PrefabName);
		BuildingController bc = obj.GetComponent<BuildingController>();
		if (bc != null)
		{
			bc.Name = Name;
		}
		return obj;
	}



	[XmlArray("Ingredients"),XmlArrayItem("Ingredient")]
	public List<PileXML> Ingredients = new List<PileXML>();

	[XmlAttribute("Hide")]
	public bool Hide = false;
	[XmlIgnore]
	public Recipe recipe;
}


