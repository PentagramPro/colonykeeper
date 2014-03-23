using UnityEngine;
using System.Collections;

public class DroneLoaderController : BaseManagedController, IStorable {

	enum Modes{
		Idle, GoLoad,DoLoad,BlockedLoad
	}

	public delegate void LoadedDelegate();

	public event LoadedDelegate OnLoaded;

	public VehicleController Vehicle;
	public IInventory Inventory;
	public int LoadAmount = 500;

	Item itemToPick;
	Modes state = Modes.Idle;
	int maxQuantityToPick;
	IInventory destinationInv;

	// Use this for initialization
	void Start () {
		if(Vehicle==null)
			throw new UnityException("Vehicle cannot be null");
		Vehicle.OnPathWalked+=OnPathWalked;
	}
	
	// Update is called once per frame
	void Update () {
		switch(state)
		{
		case Modes.DoLoad:
			int take = maxQuantityToPick -Inventory.Quantity;//destinationInv.GetItemQuantity(itemToPick);
			if(take<=0)
			{
				state = Modes.Idle;
				if(OnLoaded!=null)
					OnLoaded();
			}
			else
			{
				take = (int)Mathf.Min(take,LoadAmount*Time.smoothDeltaTime);
				int left = Inventory.Put(destinationInv.Take(itemToPick,take));
				if(left>0)
				{
					destinationInv.Put(itemToPick,left);
					if(OnLoaded!=null)
						OnLoaded();
					state = Modes.Idle;
				}
			}
			break;
		case Modes.BlockedLoad:
			IInventory inv = Vehicle.FindInventoryWith(itemToPick);
			if(inv!=null)
			{
				state = Modes.GoLoad;
				destinationInv = inv;
				Vehicle.DriveTo(inv.transform.position);
			}
			break;
		}
	}

	void OnPathWalked()
	{
		if( state == Modes.GoLoad)
		{
			state = Modes.DoLoad;
		}
	}

	public void Cancel()
	{
		if(state!=Modes.Idle)
		{
			Vehicle.Stop();
			state = Modes.Idle;
		}
	}

	public bool Load (Item itemType, int maxQuantity)
	{
		bool res = false;
		IInventory inv = Vehicle.FindInventoryWith(itemType);
		itemToPick = itemType;
		maxQuantityToPick = maxQuantity;
		if(inv!=null)
		{
			state = Modes.GoLoad;
			destinationInv = inv;
			Vehicle.DriveTo(inv.transform.position);
			res = true;
		}
		else
		{
			state = Modes.BlockedLoad;
		}
		return res;
	}

	#region IStorable implementation

	public void Save (WriterEx b)
	{

		b.WriteEnum(state);
		b.WriteLink(destinationInv);
		b.Write(maxQuantityToPick);
		b.WriteEx(itemToPick);

	}

	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		destinationInv = (IInventory)r.ReadLink(m);
		maxQuantityToPick = r.ReadInt32();
		itemToPick = (Item)r.ReadItem(m);
	}

	#endregion
}
