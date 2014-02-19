using UnityEngine;
using System.Collections.Generic;

public class JobManager : IStorable {


	public delegate void JobNotification(IJob j);
	public event JobNotification JobAdded;

	List<IJob> Jobs = new List<IJob>();
	List<IJob> BlockedJobs = new List<IJob>();
	SortedList<float, IJob> DelayedJobs = new SortedList<float, IJob>();

	const float JOB_DELAY = 10;

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
		else
		{
			Debug.LogWarning("No such job!");
		}
	}

	public void UnblockJob(IJob j)
	{
		if(BlockedJobs.Remove(j))
		{
			AddJob(j,false);
		}
		else
		{
			Debug.LogWarning("No such job!");
		}
	}

	public void DelayJob(IJob j)
	{
		float key = Time.time;
		while(DelayedJobs.ContainsKey(key))
			key+=0.001f;

		DelayedJobs.Add(key, j);
		Debug.Log("Delaying job "+j.GetHashCode());
	}

	public void UpdateJobs()
	{

		if(DelayedJobs.Count>0 && Time.time>DelayedJobs.Keys[0]+JOB_DELAY)
		{
			IJob j = DelayedJobs.Values[0];
			DelayedJobs.RemoveAt(0);
			AddJob(j,false);
			Debug.Log("Returning job to queue "+j.GetHashCode());
		}
	}

	#region IStorable implementation

	public void SaveUid(WriterEx b)
	{
	
	}
	
	public void LoadUid(Manager m, ReaderEx r)
	{
	
	}
	
	public void Save (WriterEx b)
	{

	}
	public void Load (Manager m, ReaderEx r)
	{

	}
	#endregion
}
