using System;
using UnityEngine;

public abstract class IInventory : MonoBehaviour
{
	public abstract Pile Take(float quantity);
	public abstract bool Put(Pile item);
	public abstract bool Put(Item type, float quantity);

	// returns:
	// 0 - cannot
	// 1 - can but does not prefer to take
	// 2 - prefers to take (i.e. single inventory that already contains that type of item)
	public abstract int CanTake(Item item);
	public abstract Item[] GetItemTypes();
}


