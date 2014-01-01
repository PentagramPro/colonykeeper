using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Item  {

	[XmlAttribute("ItemName")]
	public string ItemName;

	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("Description")]
	public string Description;
}
