using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class ItemProps : IStorable
{
	UidContainer uidc;


	public Color color = new Color(1,1,1);
	public Color secondaryColor = new Color(1,1,1);

	[XmlArray("Properties"),XmlArrayItem("Prop")]
	public List<Field> InitialProperties = new List<Field>();

	protected Dictionary<string,Field> Props;

	
	
	public float this[string key]
	{
		get
		{
			if(Props==null)
				BuildProps();
			if(Props.ContainsKey(key))
				return Props[key].Value;
			else return 1;

		}
		set
		{
			if(Props==null)
				BuildProps();
			if(value==1)
				Props.Remove(key);
			else
			{
				Props[key] = new Field(key,value);
			}
		}
	}
	public List<Field> PropertiesList
	{
		get
		{
			List<Field> res = new List<Field>();
			foreach(Field f in Props.Values)
				res.Add(f);
			return res;
		}
	}

	public ItemProps()
	{
		uidc = new UidContainer(this);

	}

	public bool IsSameProperties(ItemProps p)
	{
		if(p.color!=color)
			return false;

		if( (p.Props==null || Props==null) )
			return p.Props==Props;

		if(p.Props.Count!=Props.Count)
			return false;

		foreach(Field f in Props.Values)
		{
			if(p[f.Name]!=f.Value)
				return false;
		}
		return true;
	}

	public ItemProps copy()
	{
		if(Props==null)
			BuildProps();
		ItemProps res = new ItemProps();
		res.Props = new Dictionary<string, Field>();
		res.color = color;
		res.secondaryColor = secondaryColor;

		foreach(Field f in Props.Values)
			res.Props[f.Name]=new Field(f.Name,f.Value);

		return res;
	}

	void BuildProps()
	{
		Props = new Dictionary<string, Field>();
		foreach(Field f in InitialProperties)
		{
			if(f.Value!=1)
				Props[f.Name] = f;
		}
	}

	#region IStorable implementation
	public void SaveUid (WriterEx b)
	{
		uidc.SaveUid(b);
	}
	public void LoadUid (Manager m, ReaderEx r)
	{
		uidc.LoadUid(m,r);
	}
	public void Save (WriterEx b)
	{
		/*b.Write(color);
		b.Write((double)conductivity);
		b.Write((double)durability);*/
	}
	public void Load (Manager m, ReaderEx r)
	{
		/*color = r.ReadColor();
		conductivity = (float)r.ReadDouble();
		durability = (float)r.ReadDouble();*/
	}
	public int GetUID ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}


