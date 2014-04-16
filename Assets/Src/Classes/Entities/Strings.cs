using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("Strings")]
public class Strings
{
	[XmlArray("SimpleStrings"),XmlArrayItem("String")]
	public List<SimpleString> SimpleStrings;

	[XmlIgnore]
	public Dictionary<string,string> StringsByName = new Dictionary<string, string>();

	public string this[string name]
	{
		get{
			return StringsByName[name];
		}
	}

	public void Sort()
	{
		foreach(SimpleString s in SimpleStrings)
		{
			StringsByName[s.Name] = s.Text;
		}
	}

	public static Strings Load(string text)
	{
		var serializer = new XmlSerializer(typeof(Strings));
		Strings res;
		try
		{
			using(var stream = new StringReader(text))	
			{
				res =  serializer.Deserialize(stream) as Strings;	
			}
		}
		catch (FileNotFoundException)
		{
			res =  new Strings();
		}
		res.Sort();
		return res;
	}
}


