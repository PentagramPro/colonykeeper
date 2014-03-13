using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Block  {



	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("MaterialName")]
	public string MaterialName;

	[XmlIgnore]
	public bool Breakable=true;

	[XmlAttribute("Contains")]
	public string Contains;

	[XmlAttribute("Freq")]
	public float Freq=1;

	[XmlAttribute("DigSpeed")]
	public float DigSpeed=-1;
	[XmlIgnore]
	public Item ContainsItem;
}
