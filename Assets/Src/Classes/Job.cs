using UnityEngine;
using System.Collections;

public class Job  {

	public BlockController JobCell;


	IJobExecutor owner;
	public IJobExecutor Owner
	{
		get{return owner;}
	}

	public void CancelJob()
	{
		owner.CancelJob();
	}

	public void AssignJob(IJobExecutor jobOwner)
	{
		owner=jobOwner;
	}


}
