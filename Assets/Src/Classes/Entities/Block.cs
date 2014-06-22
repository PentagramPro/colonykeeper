using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Block : ObjectPrototype {

	public override GameObject Instantiate()
	{
		
		GameObject obj = base.Instantiate();
		BlockController bc = obj.GetComponent<BlockController>();
		if (bc != null)
		{
			bc.BlockProt = this;
		}
		return obj;
	}


	public bool Breakable = true;

	[XmlAttribute("Contains")]
	public string Contains;

	[XmlAttribute("Freq")]
	public float Freq=0;

	[XmlAttribute("DigSpeed")]
	public float DigSpeed=-1;
}
