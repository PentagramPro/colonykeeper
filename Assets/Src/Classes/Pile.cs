using UnityEngine;
using System.Collections;

public class Pile {

	Item itemType;
	public Item ItemType
	{
		get{return itemType;}
	}

	public float Quantity=0;

	public Pile(Item type)
	{
		itemType=type;
	}

	public Pile(Item type, float q)
	{
		itemType=type;
		Quantity=q;
	}

}
