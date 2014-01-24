using System;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Recipe
{

	public class PileXML{

		[XmlAttribute("Name")]
		public String Name;

		[XmlAttribute("Quantity")]
		public float Quantity;
	}

	[XmlAttribute("Device")]
	public string Device;

	[XmlAttribute("Name")]
	public string Name;

	[XmlArray("Ingredients"),XmlArrayItem("Ingredient")]
	public List<PileXML> Ingredients = new List<PileXML>();

	[XmlArray("Results"),XmlArrayItem("Result")]
	public List<PileXML> Results = new List<PileXML>();


}


