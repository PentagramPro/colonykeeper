using System;
using UnityEngine;
using System.Collections.Generic;

public class PileRequest
{

	Item itemType;
	int quantity;
	ItemProps properties = null;

	public int Quantity{
		get
		{
			return quantity;
		}
		set
		{
			quantity = value;
		}
	}

	public Item ItemType{
		get
		{
			return itemType;
		}
		set
		{
			itemType = value;
		}
	}

	public ItemProps Properties{
		get
		{
			return properties;
		}
		set
		{
			if(value!=null)
				properties = value.copy();
			else
				properties = null;
		}
	}
	public bool HasProperties{
		get
		{
			return properties!=null;
		}
	}



	public PileRequest (Item item, int q)
	{
		itemType = item;
		quantity = q;
	}

	public PileRequest(Item item, int q, ItemProps props)
	{
		itemType = item;
		quantity = q;
		if(props!=null)
			properties = props.copy();
	}

	public PileRequest(Pile p, int q)
	{
		itemType = p.ItemType;
		quantity = q;

		properties = p.Properties.copy();
	}

	public PileRequest(PileRequest p, int q)
	{
		itemType = p.ItemType;
		quantity = q;
		
		properties = p.Properties.copy();
	}

	public PileRequest copy()
	{
		PileRequest res = new PileRequest(itemType,quantity);
		if(properties!=null)
			res.properties = properties.copy();
		return res;
	}
}


