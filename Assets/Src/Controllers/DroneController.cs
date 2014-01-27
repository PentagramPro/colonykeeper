using UnityEngine;
using System.Collections;
using Pathfinding;
using System;


public class DroneController : VehicleController, IWorker{

	IJob currentJob;

	//////////////////////////////////////////////
	private enum Modes
	{
		Start,Idle,Work,Go,GoUnload,DoUnload,BlockedUnload,GoLoad,DoLoad,BlockedLoad
	}


	public float digAmount=5;
	public float unloadAmount = 5;

	private Modes state_int = Modes.Start;
	private Modes state{
		get{ return state_int;}
		set{
			state_int = value;
			Debug.Log(this.GetHashCode()+": setting state to " + Enum.GetName(typeof(Modes), state));
		}
	}

	IInventory destinationInv;
	float maxQuantityToPick;
	Item itemToPick;

	SingleInventory inventory;
	// Use this for initialization
	void Start () {
		base.Init ();

		inventory = GetComponent<SingleInventory>();
	}


	
	// Update is called once per frame
	new void Update () {
		if(state!=Modes.Idle)
		{
			switch(state)
			{
			case Modes.Start:
				M.JobManager.JobAdded+=OnJobAdded;
				M.BuildingsRegistry.ItemAdded+=OnBuildingAdded;
				state= Modes.Idle;
				break;
			case Modes.Work:
				currentJob.UpdateJob();
			
				break;
			case Modes.DoUnload:
				destinationInv.Put(
					inventory,
					unloadAmount*Time.smoothDeltaTime,
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
				float take = maxQuantityToPick -inventory.Quantity;//destinationInv.GetItemQuantity(itemToPick);
				if(take<=0)
				{
					state = Modes.Work;
					currentJob.OnLoaded();
				}
				else
				{
					take = Mathf.Min(take,unloadAmount*Time.smoothDeltaTime);
					float left = inventory.Put(destinationInv.Take(itemToPick,take));
					if(left>0)
					{
						destinationInv.Put(itemToPick,left);
						currentJob.OnLoaded();
						state = Modes.Work;
					}
				}
				break;
			case Modes.BlockedLoad:
				IInventory inv = FindInventoryWith(itemToPick);
				if(inv!=null)
				{
					state = Modes.GoLoad;
					destinationInv = inv;
					DriveTo(inv.transform.position);
				}
				break;
			}
		}
		base.Update ();
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
		DriveTo (dest, OnPathWalked);
	}

	public BlockController.DigResult Dig (BlockController block)
	{
		return block.Dig(inventory,digAmount*Time.smoothDeltaTime);
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

			destinationInv = FindInventoryFor(itemTypes[0]);
			if(destinationInv!=null)
			{
				state = Modes.GoUnload;
				DriveTo(destinationInv.transform.position,OnPathWalked);
			}
			else
			{
				state = Modes.BlockedUnload;
			}
		}

	}

	public void Load (Item itemType, float maxQuantity)
	{
		IInventory inv = FindInventoryWith(itemType);
		itemToPick = itemType;
		maxQuantityToPick = maxQuantity;
		if(inv!=null)
		{
			state = Modes.GoLoad;
			destinationInv = inv;
			DriveTo(inv.transform.position,OnPathWalked);
		}
		else
		{
			state = Modes.BlockedLoad;
		}
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

	public void Pick(IInventory inv, Item itemType, float quantity)
	{
		inventory.Put(inv.Take(itemType,quantity));
	}

	#endregion
}
