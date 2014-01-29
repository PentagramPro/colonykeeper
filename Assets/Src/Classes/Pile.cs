using UnityEngine;
using System.Collections;

public class Pile {

	Item itemType;
	public Item ItemType
	{
		get{return itemType;}
	}

	private int quantity=0;
	public int Quantity{
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


	public Pile(Item type, int q)
	{
		itemType=type;
		Quantity=q;
	}

}
