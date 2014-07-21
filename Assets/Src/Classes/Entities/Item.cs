using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Item : IListItem
{


	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("Description")]
	public string Description;

	[XmlAttribute("Class")]
	public string ItemClass;


	public ItemProps BaseProperties = new ItemProps();

	public bool IsOfClass(string cls)
	{
		if(string.IsNullOrEmpty(ItemClass))
			return string.IsNullOrEmpty(cls);
		return ItemClass.StartsWith(cls);
	}

    public string GetName()
    {
        return Name;
    }
}
