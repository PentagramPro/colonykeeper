using System;

public class SupplyJob : IJob
{
	enum Modes
	{
		Start, Load,Go,Feed,End
	}

	Modes state = Modes.Start;
	PileRequest itemToPick;

	IInventory inventoryToSupply;
	BuildingController building;

	/*public Item ItemType{
		get{ return itemToPick.ItemType;}
	}*/
	public PileRequest Request
	{
		get{
			return itemToPick;
		}
	}

	public SupplyJob()
	{
	}

	public SupplyJob (JobManager jobManager, ICustomer customer,BuildingController target, IInventory targetInventory, PileRequest item) 
		: base(jobManager, customer)
	{
		itemToPick = item;
		inventoryToSupply = targetInventory;
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
		worker.DriveTo(building.Position, building.collider);
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
			if(worker.Load(itemToPick))
				state = Modes.Load;
			else
			{
				DelayThisJob();
				state = Modes.Start;
			}
			break;
		}
	}


	#endregion

	public override void Save (WriterEx b)
	{
		base.Save (b);
		/*b.WriteEnum(state);
		b.WriteLink(building);
		b.Write(maxQuantity);
		b.WriteEx(itemToPick);
		b.WriteLink(inventoryToSupply);*/
	}

	public override void Load (Manager m, ReaderEx r)
	{
		base.Load (m, r);
		/*state = (Modes)r.ReadEnum(typeof(Modes));
		building = (BuildingController)r.ReadLink(m);
		maxQuantity = r.ReadInt32();
		itemToPick = r.ReadItem(m);
		inventoryToSupply = (IInventory)r.ReadLink(m);
		*/

	}
}


