using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Item  {


	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("Description")]
	public string Description;

	[XmlAttribute("Class")]
	public string ItemClass;

	public bool IsOfClass(string cls)
	{
		if(string.IsNullOrEmpty(ItemClass))
			return string.IsNullOrEmpty(cls);
		return ItemClass.StartsWith(cls);
	}
}
