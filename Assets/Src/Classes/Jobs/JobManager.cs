using UnityEngine;
using System.Collections.Generic;

public class JobManager  {


	public delegate void JobNotification(IJob j);
	public event JobNotification JobAdded;

	List<IJob> Jobs = new List<IJob>();
	List<IJob> BlockedJobs = new List<IJob>();

	public void AddJob(IJob j, bool isBlocked)
	{
		if(isBlocked)
			BlockedJobs.Add(j);
		else
		{
			Jobs.Add(j);
			if(JobAdded!=null)
				JobAdded(j);
		}
	}

	public IJob FindJob()
	{
	
		var it = Jobs.GetEnumerator();
		if(it.MoveNext())
			return it.Current;
			
		return null;
	}
	public bool AssignJob(IJob j,IWorker owner)
	{
		if(j.Worker!=null || !Jobs.Contains(j))
			return false;
		
		if(Jobs.Remove(j))
		{
			j.AssignTo(owner);
			return true;
		}
		return false;
	}


	public void CompleteJob(IJob j)
	{

	}

	public bool RemoveJob(IJob j)
	{
		return Jobs.Remove(j) || BlockedJobs.Remove(j);
		
	}

	public void BlockJob(IJob j)
	{
		if(Jobs.Remove(j))
		{
			BlockedJobs.Add(j);
		}
	}

	public void UnblockJob(IJob j)
	{
		if(BlockedJobs.Remove(j))
		{
			AddJob(j,false);
		}
	}


}
