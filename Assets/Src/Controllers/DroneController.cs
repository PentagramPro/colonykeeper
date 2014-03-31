using UnityEngine;
using System.Collections;
using Pathfinding;
using System;


public class DroneController : BaseManagedController, IWorker, IStorable{

	public VehicleController vehicleController;
	public DroneLoaderController loaderController;
	HullController hull;

	IJob currentJob;

	//////////////////////////////////////////////
	private enum Modes
	{
		Start,Idle,Runaway,Work,Go,GoUnload,DoUnload,BlockedUnload,Load
	}


	public int digAmount=1000;
	public int unloadAmount = 500;

	private Modes state_int = Modes.Start;
	private Modes state{
		get{ return state_int;}
		set{
			state_int = value;
			Debug.Log(this.GetHashCode()+": setting state to " + Enum.GetName(typeof(Modes), state));
		}
	}

	IInventory destinationInv;



	IInventory inventory;
	// Use this for initialization
	void Start () {

		if(vehicleController==null)
			throw new UnityException("Vehicle exception must not be null");

		if(loaderController==null)
			throw new UnityException("Drone loader controller must not be null");
		inventory = GetComponent<IInventory>();
		hull = GetComponent<HullController>();

		vehicleController.OnPathWalked+=OnPathWalked;
		hull.OnUnderAttack +=OnUnderAttack;

		loaderController.OnLoaded+=OnDroneLoaded;

		M.JobManager.JobAdded+=OnJobAdded;
		M.BuildingsRegistry.ItemAdded+=OnBuildingAdded;
	}

	void OnDestroy()
	{
		M.JobManager.JobAdded-=OnJobAdded;
		M.BuildingsRegistry.ItemAdded-=OnBuildingAdded;
		if (currentJob != null)
			currentJob.Cancel();
	}

	
	// Update is called once per frame
	void Update ()
	{
		if(state!=Modes.Idle)
		{
			switch(state)
			{
			case Modes.Start:

				state= Modes.Idle;
				break;
			case Modes.Work:
				currentJob.UpdateJob();
			
				break;
			case Modes.DoUnload:
				{
					Item itemToUnload = inventory.GetItemTypes()[0];
					destinationInv.Put(
						inventory,
						(int)(unloadAmount*Time.smoothDeltaTime),
						itemToUnload);
					
					if(inventory.Quantity==0)
					{
						state = Modes.Work;
						currentJob.OnUnloaded();
					}
					else if(destinationInv.IsFull())
					{
						Unload();
					}
					else if(!destinationInv.CanTake(itemToUnload))
					{
						Unload();
					}
				}
				break;

			}
		}

	}



	void OnBuildingAdded(object sender, EventArgs e)
	{
		if(state==Modes.BlockedUnload)
			Unload();
	}

	void OnPathWalked()
	{
		Debug.Log("OnPathWalked, state = " + Enum.GetName(typeof(Modes), state));
		if (state == Modes.Go)
		{
			state = Modes.Work;
			currentJob.OnDriven();
		}
		else if (state == Modes.GoUnload)
		{
			state = Modes.DoUnload;
		}
		else if(state == Modes.Runaway)
		{
			state = Modes.Idle;
		}
			
		    
	}


	void OnDroneLoaded()
	{
		state = Modes.Work;
		if(currentJob!=null)
			currentJob.OnLoaded();

	}

	void OnJobAdded(IJob j)
	{
		if(state!=Modes.Idle)
			return;

		if(	M.JobManager.AssignJob(j,this))
		{
			state = Modes.Work;
			currentJob=j;
		}
	}

	void OnUnderAttack()
	{
		if(state == Modes.Idle)
		{
			state = Modes.Runaway;
			vehicleController.DriveTo(M.defenceController.GetComponent<BuildingController>().Position);
		}
	}

	#region IWorker implementation
	public void SetCallbacks (IJob job)
	{
		currentJob = job;
	}

	public void CancelCurrentJob ()
	{
		currentJob = null;
		loaderController.Cancel();
		state = Modes.Idle;
	}

	public void DriveTo (Vector3 dest)
	{
		state = Modes.Go;
		vehicleController.DriveTo (dest);
	}

	public BlockController.DigResult Dig (BlockController block)
	{
		return block.Dig(inventory);
	}

	public bool Unload ()
	{
		bool res = true;
		if(inventory.Quantity==0)
		{
			state = Modes.Work;
			currentJob.OnUnloaded();
		}
		else
		{
			Item[] itemTypes = inventory.GetItemTypes();

			destinationInv = M.FindInventoryFor(itemTypes[0]);
			if(destinationInv!=null)
			{
				state = Modes.GoUnload;
				vehicleController.DriveTo(destinationInv.transform.position);
			}
			else
			{
				//state = Modes.BlockedUnload;
				//res = false;
				M.DisplayMessage(string.Format(M.S["Message.NoStorage"],itemTypes[0].Name));
				inventory.DropCrate();
				state = Modes.Work;
				currentJob.OnUnloaded();
				res = false;
			}
		}
		return res;
	}

	public bool Load (Item itemType, int maxQuantity)
	{
		state = Modes.Load;
		return loaderController.Load(itemType,maxQuantity);
	}

	public void OnJobCompleted ()
	{
		if (inventory.Quantity > 0)
			inventory.DropCrate();
		
		state = Modes.Idle;
		currentJob = M.JobManager.FindJob();
		if (currentJob != null)
		{
			if (M.JobManager.AssignJob(currentJob, this))
			{
				state = Modes.Work;
			}
		}
		
	}

	public void Feed(IInventory inv)
	{
		inv.Put(inventory,inventory.Quantity,inventory.GetItemTypes()[0]);
	}

	public void Pick(IInventory inv, Item itemType, int quantity)
	{
		inventory.Put(inv.Take(itemType,quantity));
	}

	#endregion

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.WriteMagic();

		b.WriteEnum(state);
		b.WriteLink(currentJob);
		b.WriteLink(destinationInv);
	}

	public void Load (Manager m, ReaderEx r)
	{
		r.CheckMagic();

		state = (Modes)r.ReadEnum(typeof(Modes));
		currentJob = (IJob)r.ReadLink(m);
		destinationInv = (IInventory)r.ReadLink(m);
	}

	#endregion
}
