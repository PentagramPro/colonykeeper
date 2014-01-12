using UnityEngine;
using System.Collections;

public class Job : IJob {

	public BlockController JobCell;


	IWorker owner;
	public IWorker Owner
	{
		get{return owner;}
	}

	public void CancelJob()
	{
		owner.CancelJob();
	}

	public void AssignJob(IWorker jobOwner)
	{
		owner=jobOwner;
	}


}
