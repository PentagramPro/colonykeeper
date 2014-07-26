using System.Xml.Serialization;

public class PropertyTransfer
{
	[XmlAttribute("Src")]
	public string SourceProperty="";

	[XmlAttribute("Dst")]
	public string DestinationProperty="";

	[XmlAttribute("X")]
	public float Multiplier=1;

	public bool EmptyPropertyNames
	{
		get
		{
			return SourceProperty=="" && DestinationProperty=="";
		}
	}

	public bool EqualPropertyNames
	{
		get
		{
			return SourceProperty=="" || DestinationProperty=="";
		}
	}
}


