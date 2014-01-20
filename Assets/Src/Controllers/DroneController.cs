using UnityEngine;
using System.Collections;
using Pathfinding;
using System;


public class DroneController : VehicleController, IWorker{
	private enum Modes
	{
		Init,Idle,Go,Work,Transport,Unload,Blocked
	}


	public float digAmount=5;
	public float unloadAmount = 5;

	private Modes state = Modes.Init;

	IInventory destinationInv;

	SingleInventory inventory;
	// Use this for initialization
	void Start () {
		base.Init ();

		inventory = GetComponent<SingleInventory>();
	}

	Job digJob;
	
	// Update is called once per frame
	void Update () {
		if(state!=Modes.Idle)
		{
			switch(state)
			{
			case Modes.Init:
				M.JobManager.DigJobAdded+=OnDigJobAdded;
				M.BuildingsRegistry.ItemAdded+=OnBuildingAdded;
				state= Modes.Idle;
				break;
			case Modes.Go:
				break;
			case Modes.Work:
				DoWork();
			
				break;
			case Modes.Unload:
				destinationInv.Put(inventory,unloadAmount*Time.smoothDeltaTime,inventory.GetItemTypes()[0]);

				if(inventory.Quantity==0)
				{
					if(digJob==null)
						FindAnotherJob();
					else
					{
						state = Modes.Go;
						DriveTo (digJob.JobCell.Position, OnPathWalked);
					}
				}
				else if(destinationInv.IsFull())
					DoTransport();
				break;
			}
		}
		base.Update ();
	}

	void DoTransport()
	{
		state=Modes.Transport;
		bool found=false;
		foreach (BlockController b in M.BuildingsRegistry.Keys) 
		{
			BuildingController building = M.BuildingsRegistry[b];

			IInventory i = building.GetComponent<IInventory>();
			if(i==null)
				continue;

			Item[] itemTypes = inventory.GetItemTypes();

			if(i.CanPut(itemTypes[0])>0)
			{
				destinationInv=i;
				DriveTo(i.transform.position,OnPathWalked);
				found=true;
				break;
			}

		}
		if (!found)
			state = Modes.Blocked;
	}

	void CompleteJob()
	{
		M.JobManager.CompleteDigJob(digJob);
		digJob=null;
		state = Modes.Idle;
	}

	void FindAnotherJob()
	{
		Job j = M.JobManager.FindDigJob();
		if(j==null)
			state = Modes.Idle;
		else
			AssignJob(j);
	}

	void DoWork()
	{
		/*M.JobManager.CompleteDigJob(digJob);
		if(inventory.Quantity>0)
			DoTransport();
		else
		{ 
			FindAnotherJob();
		}*/

		switch(digJob.JobCell.Dig(inventory,digAmount*Time.smoothDeltaTime))
		{
		case BlockController.DigResult.Finished:
			CompleteJob();
			FindAnotherJob();
			break;
		case BlockController.DigResult.DestinationFull:
			DoTransport();
			break;
		case BlockController.DigResult.NotFinished:
			break;
		case BlockController.DigResult.CannotDig:
			CompleteJob();
			FindAnotherJob();
			throw new UnityException("Dig error!");

		}
	}

	void OnBuildingAdded(object sender, EventArgs e)
	{
		if((state==Modes.Idle || state==Modes.Blocked) && inventory.Quantity>0)
			DoTransport();
	}

	void OnPathWalked()
	{
		if (state == Modes.Go)
			state = Modes.Work;
		else if (state == Modes.Transport)
			state = Modes.Unload;
		    
	}


	void AssignJob(Job j)
	{
		state = Modes.Go;
		M.JobManager.AssignDigJob(j,this);
		digJob=j;
		DriveTo (j.JobCell.Position, OnPathWalked);

	}
	void OnDigJobAdded(Job j)
	{
		if(j.Owner==null && state==Modes.Idle)
		{
			AssignJob(j);
		}
	}
	#region IWorker implementation

	public void CancelJob ()
	{
		state = Modes.Idle;
	}

	#endregion


}
