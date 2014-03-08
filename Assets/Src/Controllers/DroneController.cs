using UnityEngine;
using System.Collections;
using Pathfinding;
using System;


public class DroneController : BaseManagedController, IWorker, IStorable{

	public VehicleController vehicleController;
	HullController hull;

	IJob currentJob;

	//////////////////////////////////////////////
	private enum Modes
	{
		Start,Idle,Runaway,Work,Go,GoUnload,DoUnload,BlockedUnload,GoLoad,DoLoad,BlockedLoad
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
	int maxQuantityToPick;
	Item itemToPick;

	IInventory inventory;
	// Use this for initialization
	void Start () {

		if(vehicleController==null)
			throw new UnityException("Vehicle exception must not be null");
		inventory = GetComponent<IInventory>();
		hull = GetComponent<HullController>();

		vehicleController.OnPathWalked+=OnPathWalked;
		hull.OnUnderAttack +=OnUnderAttack;

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
				destinationInv.Put(
					inventory,
					(int)(unloadAmount*Time.smoothDeltaTime),
					inventory.GetItemTypes()[0]);

				if(inventory.Quantity==0)
				{
					state = Modes.Work;
					currentJob.OnUnloaded();
				}
				else if(destinationInv.IsFull())
				{
					Unload();
				}
				break;
			case Modes.DoLoad:
				int take = maxQuantityToPick -inventory.Quantity;//destinationInv.GetItemQuantity(itemToPick);
				if(take<=0)
				{
					state = Modes.Work;
					currentJob.OnLoaded();
				}
				else
				{
					take = (int)Mathf.Min(take,unloadAmount*Time.smoothDeltaTime);
					int left = inventory.Put(destinationInv.Take(itemToPick,take));
					if(left>0)
					{
						destinationInv.Put(itemToPick,left);
						currentJob.OnLoaded();
						state = Modes.Work;
					}
				}
				break;
			case Modes.BlockedLoad:
				IInventory inv = vehicleController.FindInventoryWith(itemToPick);
				if(inv!=null)
				{
					state = Modes.GoLoad;
					destinationInv = inv;
					DriveTo(inv.transform.position);
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
		else if( state == Modes.GoLoad)
		{
			state = Modes.DoLoad;
		}
		else if(state == Modes.Runaway)
		{
			state = Modes.Idle;
		}
			
		    
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
		state = Modes.Idle;
	}

	public void DriveTo (Vector3 dest)
	{
		state = Modes.Go;
		vehicleController.DriveTo (dest);
	}

	public BlockController.DigResult Dig (BlockController block)
	{
		return block.Dig(inventory,(int)(digAmount*Time.smoothDeltaTime));
	}

	public void Unload ()
	{
		if(inventory.Quantity==0)
		{
			state = Modes.Work;
			currentJob.OnUnloaded();
		}
		else
		{
			Item[] itemTypes = inventory.GetItemTypes();

			destinationInv = vehicleController.FindInventoryFor(itemTypes[0]);
			if(destinationInv!=null)
			{
				state = Modes.GoUnload;
				vehicleController.DriveTo(destinationInv.transform.position);
			}
			else
			{
				state = Modes.BlockedUnload;
			}
		}

	}

	public bool Load (Item itemType, int maxQuantity)
	{
		bool res = false;
		IInventory inv = vehicleController.FindInventoryWith(itemType);
		itemToPick = itemType;
		maxQuantityToPick = maxQuantity;
		if(inv!=null)
		{
			state = Modes.GoLoad;
			destinationInv = inv;
			vehicleController.DriveTo(inv.transform.position);
			res = true;
		}
		else
		{
			state = Modes.BlockedLoad;
		}
		return res;
	}

	public void OnJobCompleted ()
	{
		state = Modes.Idle;
		currentJob = M.JobManager.FindJob();
		if(currentJob!=null)
		{
			if(	M.JobManager.AssignJob(currentJob,this))
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
		b.Write(maxQuantityToPick);
		b.WriteEx(itemToPick);
	}

	public void Load (Manager m, ReaderEx r)
	{
		r.CheckMagic();
		state = (Modes)r.ReadEnum(typeof(Modes));

		currentJob = (IJob)r.ReadLink(m);
		destinationInv = (IInventory)r.ReadLink(m);
		maxQuantityToPick = r.ReadInt32();
		itemToPick = (Item)r.ReadItem(m);
	}

	#endregion
}
