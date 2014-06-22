using UnityEngine;
using System;
using System.Xml.Serialization;

public class VehicleProt : ObjectPrototype
{

	public Recipe Recipe;

	public GameObject Instantiate(Transform parent)
	{
		GameObject obj = base.Instantiate();

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
