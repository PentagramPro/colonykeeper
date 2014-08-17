using System.Xml.Serialization;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ObjectPrototype : IListItem
{
	[XmlAttribute("Name")]
	public string Name;
	
	[XmlAttribute("PrefabName")]
	public string PrefabName;

	public abstract GameObject Instantiate();

	protected GameObject Instantiate(string folder, string name)
	{
		if(string.IsNullOrEmpty(PrefabName))
			throw new UnityException("Cannot execute Instantiate method for Block with empty PrefabName");
		
		GameObject obj = Resources.Load<GameObject>(string.Format("Prefabs/{0}/{1}",folder,name));
		
		if(obj==null)
			throw new UnityException("Cannot find prefab with name: "+PrefabName);

		#if UNITY_EDITOR
		obj = (GameObject)PrefabUtility.InstantiatePrefab(obj);
		#else
		obj = (GameObject)GameObject.Instantiate(obj);
		#endif
		return obj;
	}

    public string GetName()
    {
        return Name;
    }
}


