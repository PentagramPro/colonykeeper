using UnityEngine;
using System.Collections;

public class SiloController : BaseManagedController, IInventory {

	SingleInventory inventory = new SingleInventory();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IInventory implementation

	public Pile Take (float quantity)
	{
		inventory.Take (quantity);
	}

	public bool Put (Pile item)
	{
		inventory.Put (item);
	}

	public bool Put (Item type, float quantity)
	{
		inventory.Put (type, quantity);
	}

	public int CanTake(Item i)
	{
		return inventory.CanTake (i);
	}

	#endregion
}
