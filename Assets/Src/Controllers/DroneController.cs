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
	public enum Modes
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
			//Debug.Log(this.GetHashCode()+": setting state to " + Enum.GetName(typeof(Modes), state));
		}
	}
    public Modes State
    {
        get
        {
            return state_int;
        }
    }

	IInventory destinationInv;

	FloatingTextController digText;

	IInventory inventory;
	// Use this for initialization
	void Start () {

		if(vehicleController==null)
			throw new UnityException("VehicleProt exception must not be null");

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

	void OnFloatingTextDestroyed()
	{
		digText = null;
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
					Pile pileToUnload = inventory.FirstPile;
					destinationInv.Put(
						inventory,
						pileToUnload,
						(int)(unloadAmount*Time.smoothDeltaTime));
					
					string line = string.Format("{0:0.00} {1}", inventory.Quantity/100.0f,pileToUnload.GetName());
					FloatingTextController.LastingText(this,hull.Center,line);

					if(inventory.Quantity==0)
					{
						FloatingTextController.ResetText(this);
						state = Modes.Work;
						currentJob.OnUnloaded();
					}
					else if(destinationInv.IsFull())
					{
						FloatingTextController.ResetText(this);
						Unload();
					}
					else if(!destinationInv.CanTake(new PileRequest(pileToUnload,0),false))
					{
						FloatingTextController.ResetText(this);
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
            if(currentJob!=null)
			    currentJob.OnDriven();
            else
                state = Modes.Idle;
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

	void OnUnderAttack(Transform attacker)
	{
		if(state == Modes.Idle && M.defenceController!=null)
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
		inventory.DropCrate();
		state = Modes.Idle;
	}

	public void DriveTo (Vector3 dest)
	{
		DriveTo(dest,null);
	}
	public void DriveTo (Vector3 dest, Collider collider)
	{
		state = Modes.Go;
		vehicleController.DriveTo (dest, collider);
	}

	public BlockController.DigResult Dig (BlockController block)
	{
		int delta = inventory.Quantity;
		BlockController.DigResult res =  block.Dig(inventory);
		delta=inventory.Quantity-delta;
		if(delta>0)
		{
			string line = string.Format("+{0:0.00} {1}", inventory.Quantity/100.0f,inventory.GetItemTypes()[0].Name);
			FloatingTextController.LastingText(this,hull.Center,line);

		}
		if(res!=BlockController.DigResult.NotFinished)
			FloatingTextController.ResetText(this);

		return res;
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
				vehicleController.DriveTo(destinationInv.transform.position, destinationInv.collider);
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

	public bool Load (PileRequest loadRequest)
	{
		state = Modes.Load;
		return loaderController.Load(loadRequest);
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
		if(inventory.Quantity>0)
			inv.Put(inventory, inventory.FirstPile,inventory.Quantity);
	}

	public void Pick(IInventory inv,  PileRequest pickRequest)
	{
	    Pile p = inv.Take(pickRequest);
	    //FloatingTextController.SpawnText(">"+p.StringQuantity+" "+p.GetName(),transform.position);
		inventory.Put(p);
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
