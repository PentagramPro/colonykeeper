using UnityEngine;
using System.Collections;

public class Pile {

	Item itemType;
	public Item ItemType
	{
		get{return itemType;}
	}

	private float quantity=0;
	public float Quantity{
		get{
			return quantity;
		}
		set{
			quantity = value;
		}
	}

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
