using System;

public interface IInventory
{
	Pile Take(float quantity);
	bool Put(Pile item);
	bool Put(Item type, float quantity);

	// returns:
	// 0 - cannot
	// 1 - can but does not prefer to take
	// 2 - prefers to take (i.e. single inventory that already contains that type of item)
	int CanTake(Item item);
}


