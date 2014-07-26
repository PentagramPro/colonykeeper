using System.Xml.Serialization;
using System.Collections.Generic;

public class Ingredient
{
	[XmlAttribute("Name")]
	public string Name;
	[XmlAttribute("Quantity")]
	public int Quantity = 100;
	// list of property names. These properties will be taken from this ingredient during craft
	[XmlArray("Props"),XmlArrayItem("P")]
	public List<PropertyTransfer> Properties = new List<PropertyTransfer>();

	// each of the items from the list can be used as an ingredient
	[XmlIgnore]
	public List<Item> Items = new List<Item>();


	[XmlIgnore]
	public string ClassName;

	public bool Contains(Item item)
	{
		return Items.Contains(item);
	}

}

