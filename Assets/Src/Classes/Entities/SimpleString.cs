using System.Xml;
using System.Xml.Serialization;


public class SimpleString
{
	[XmlAttribute("Name")]
	public string Name;

	[XmlText]
	public string Text;
}

