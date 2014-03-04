using UnityEngine;
using System;
using System.Xml.Serialization;

public class Vehicle
{
	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("PrefabName")]
	public string PrefabName;

	public Recipe Recipe;

	public GameObject Instantiate()
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for vehicle with empty PrefabName");
		
		GameObject obj = Resources.Load<GameObject>("Prefabs/Vehicles/"+PrefabName);
		
		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);
		
		obj = (GameObject)GameObject.Instantiate(obj);
		VehicleController vc = obj.GetComponent<VehicleController>();
		if (vc != null)
		{
			vc.Prototype = this;
		}
		return obj;
	}
}
