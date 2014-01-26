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

	public SupplyJob (JobManager jobManager,BlockController target, IInventory targetInventory, Item item, float quantity) 
		: base(jobManager,target)
	{
		itemToPick = item;
		inventoryToSupply = targetInventory;
		maxQuantity = quantity;
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
		worker.DriveTo(BlockController.Position);
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


