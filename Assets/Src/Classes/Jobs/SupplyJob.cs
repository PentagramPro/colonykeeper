using System;

public class SupplyJob : IJob
{
	enum Modes
	{
		Start, Load,Go,Feed,End
	}

	Modes state = Modes.Start;
	Item itemToPick;
	float maxQuantity;
	IInventory inventoryToSupply;
	BuildingController building;

	public Item ItemType{
		get{ return itemToPick;}
	}

	public SupplyJob (JobManager jobManager, ICustomer customer,BuildingController target, IInventory targetInventory, Item item, float quantity) 
		: base(jobManager, customer)
	{
		itemToPick = item;
		inventoryToSupply = targetInventory;
		maxQuantity = quantity;
		building = target;
	}

	#region implemented abstract members of IJob
	public override void OnDriven ()
	{
		worker.Feed(inventoryToSupply);
		state = Modes.End;
		Complete();
	}
	public override void OnLoaded ()
	{
		worker.DriveTo(building.Position);
		state = Modes.Go;
	}
	public override void OnUnloaded ()
	{

	}
	public override void UpdateJob ()
	{
		switch(state)
		{
		case Modes.Start:
			worker.Load(itemToPick,maxQuantity);
			state = Modes.Load;
			break;
		}
	}
	#endregion


}


