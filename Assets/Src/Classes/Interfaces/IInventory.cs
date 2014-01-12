using System;

public interface IInventory
{
	Pile Take(float quantity);
	bool Put(Pile item);
}


