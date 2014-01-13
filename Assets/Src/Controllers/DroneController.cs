using UnityEngine;
using System.Collections;
using Pathfinding;

public class DroneController : VehicleController, IWorker, IInventory{
	private enum Modes
	{
		Init,Idle,Go,Work
	}


	public float digAmount=5;

	private Modes state = Modes.Init;

	SingleInventory inventory = new SingleInventory();
	// Use this for initialization
	void Start () {
		base.Init ();

		//controller = GetComponent<CharacterController>();
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
				state= Modes.Idle;
				break;
			case Modes.Go:
				break;
			case Modes.Work:
				DoWork();

				break;
			}
		}
		base.Update ();
	}


	void DoWork()
	{
		if(digJob.JobCell.Dig(inventory,digAmount*Time.smoothDeltaTime))
		{
			M.JobManager.CompleteDigJob(digJob);
			Job j = M.JobManager.FindDigJob();
			if(j==null)
				state = Modes.Idle;
			else
				AssignJob(j);
			

		}
	}


	void OnPathWalked()
	{
		if(state!=Modes.Idle)
			state = Modes.Work;
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

	#region IInventory implementation

	public Pile Take (float quantity)
	{
		return inventory.Take(quantity);
	}

	public bool Put (Pile item)
	{
		return inventory.Put(item);
	}

	public bool Put(Item i, float q)
	{
		return inventory.Put(i,q);
	}
	#endregion
}
