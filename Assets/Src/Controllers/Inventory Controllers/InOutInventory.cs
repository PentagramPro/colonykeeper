using UnityEngine;
using System.Collections.Generic;

public class InOutInventory : IInventory {


	Pile inPile,outPile;

	public float MaxInQuantity = 5;
	public float MaxOutQuantity = 5;
	
	
	
	public float InQuantity{
		get{ return inPile==null?0:inPile.Quantity;}
	}
	public float OutQuantity{
		get{ return outPile==null?0:outPile.Quantity;}
	}

	#region IInventory implementation
	
	public override Pile Take (Item itemType, float quantity)
	{
		
		if(quantity<0)
			throw new UnityException("Negative values are not allowed!");
		
		float q = Mathf.Min(quantity,outPile.Quantity);
		
		if (outPile == null || outPile.ItemType!=itemType)
			return null;
		
		
		else if(outPile.Quantity==q)
		{
			Pile res=outPile;
			outPile=null;
			return res;
		}
		else
		{
			outPile.Quantity-=q;
			return new Pile(outPile.ItemType,q);
		}
	}
	
	public override float Put (Item type, float quantity)
	{
		if(inPile==null)
			inPile = new Pile(type);
		else if(inPile.ItemType!=type)
			return quantity;
		
		float free = MaxInQuantity -inPile.Quantity;
		
		inPile.Quantity+=Mathf.Min(quantity,free);
		return Mathf.Max(0,quantity-free);
	}
	
	
	
	public override int CanPut(Item item)
	{
		if (inPile == null)
			return 1;
		
		if (inPile.ItemType == item && inPile.Quantity<MaxInQuantity)
			return 2;
		
		return 0;
	}
	
	public override bool CanTake (Item item)
	{
		return outPile!=null && outPile.ItemType==item;
	}
	
	public override Item[] GetItemTypes ()
	{
		Item[] res = new Item[outPile==null?0:1];
		if(res.Length>0)
			res [0] = outPile.ItemType;
		return res;
	}
	
	public override bool IsFull()
	{
		return inPile!=null && inPile.Quantity>=MaxInQuantity ;
	}
	
	#endregion

	
	
	

}
