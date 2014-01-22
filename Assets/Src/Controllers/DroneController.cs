using UnityEngine;
using System.Collections;
using Pathfinding;
using System;


public class DroneController : VehicleController, IWorker{

	IJob currentJob;

	//////////////////////////////////////////////
	private enum Modes
	{
		Start,Idle,Work,Go,Unload,Feed,Blocked
	}


	public float digAmount=5;
	public float unloadAmount = 5;

	private Modes state = Modes.Start;

	IInventory destinationInv;

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
			case Modes.Feed:
				destinationInv.Put(inventory,unloadAmount*Time.smoothDeltaTime,inventory.GetItemTypes()[0]);

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
			}
		}
		base.Update ();
	}



	void OnBuildingAdded(object sender, EventArgs e)
	{
		if(state==Modes.Blocked)
			Unload();
	}

	void OnPathWalked()
	{
		if (state == Modes.Go)
		{
			state = Modes.Work;
			currentJob.OnDriven();
		}
		else if (state == Modes.Unload)
		{
			state = Modes.Feed;
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

	public BlockController.DigResult Dig ()
	{
		return currentJob.BlockController.Dig(inventory,digAmount*Time.smoothDeltaTime);
	}

	public void Unload ()
	{
		Item[] itemTypes = inventory.GetItemTypes();

		destinationInv = FindInventoryFor(itemTypes[0]);
		if(destinationInv==null)
		{
			state = Modes.Unload;
			DriveTo(destinationInv.transform.position,OnPathWalked);
		}
		else
		{
			state = Modes.Blocked;
		}

	}

	public void Load (Item itemType, float maxQuantity)
	{

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
	#endregion
}
