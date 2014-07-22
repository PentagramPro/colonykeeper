using UnityEngine;
using System.Xml.Serialization;

public class Field : IListItem
{
	[XmlAttribute("Name")]
	public string Name = "";

	[XmlAttribute("Val")]
	public float Value = 1;

	public Field()
	{
	}

	public Field(string n, float v)
	{
		Name = n;
		Value = v;
	}

	#region IListItem implementation

	public string GetName ()
	{
		return Name;
	}

	#endregion
}


