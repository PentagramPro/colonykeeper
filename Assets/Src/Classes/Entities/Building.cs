using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;

public class Building 
{
	public GameObject Instantiate()
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for Block with empty PrefabName");
		
		GameObject obj = Resources.Load<GameObject>("Prefabs/Blocks/"+PrefabName);
		
		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);
		

		return obj;
	}


	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("PrefabName")]
	public string PrefabName;

}


