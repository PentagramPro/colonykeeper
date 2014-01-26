using System;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Recipe
{



	[XmlAttribute("Device")]
	public string Device;

	[XmlAttribute("Name")]
	public string Name;

	[XmlArray("Ingredients"),XmlArrayItem("Ingredient")]
	public List<PileXML> Ingredients = new List<PileXML>();

	[XmlArray("Results"),XmlArrayItem("Result")]
	public List<PileXML> Results = new List<PileXML>();

	[XmlIgnore]
	public List<Pile> IngredientsLinks = new List<Pile>();

	[XmlIgnore]
	public List<Pile> ResultsLinks = new List<Pile>();

	public override string ToString ()
	{
		return Name;
	}

}


