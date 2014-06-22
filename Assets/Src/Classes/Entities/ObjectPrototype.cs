using System.Xml.Serialization;
using System;
using UnityEngine;

public class ObjectPrototype
{
	[XmlAttribute("Name")]
	public string Name;
	
	[XmlAttribute("PrefabName")]
	public string PrefabName;

	public virtual GameObject Instantiate()
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for Block with empty PrefabName");
		
		GameObject obj = Resources.Load<GameObject>("Prefabs/Buildings/"+PrefabName);
		
		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);
		
		obj = (GameObject)GameObject.Instantiate(obj);

		return obj;
	}
}


