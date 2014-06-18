using UnityEngine;
using System;
using System.Xml.Serialization;

public class VehicleProt
{
	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("PrefabName")]
	public string PrefabName;

	public Recipe Recipe;

	public GameObject Instantiate(Transform parent)
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for vehicle with empty PrefabName");
		
		GameObject obj = Resources.Load<GameObject>("Prefabs/Vehicles/"+PrefabName);
		
		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);
		
		obj = (GameObject)GameObject.Instantiate(obj);
		obj.transform.parent = parent;
		VehicleController vc = obj.GetComponent<VehicleController>();
		if (vc != null)
		{
			//vc.Prototype = this;
			vc.Name = Name;
		}
		return obj;
	}
}
