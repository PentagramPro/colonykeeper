using UnityEngine;
using System.Collections.Generic;

public class JobManager  {


	public delegate void JobNotification(Job j);
	public event JobNotification DigJobAdded;

	Dictionary<BlockController,Job> DigJobs = new Dictionary<BlockController, Job>();
	Dictionary<BlockController,Job> AssignedDigJobs = new Dictionary<BlockController, Job>();

	public void AddDigJob(BlockController c)
	{
		Job j = new Job();
		j.JobCell=c;
		DigJobs.Add(c,j);
		if(DigJobAdded!=null)
			DigJobAdded(j);
	}

	public Job FindDigJob()
	{
	
		var it = DigJobs.GetEnumerator();
		if(it.MoveNext())
			return it.Current.Value;

	
			
		return null;
	}
	public void AssignDigJob(Job j,IJobExecutor owner)
	{
		if(j.Owner==null)
		{
			DigJobs.Remove(j.JobCell);
			AssignedDigJobs.Add(j.JobCell,j);
			j.AssignJob(owner);
		}
	}


	public void CompleteDigJob(Job j)
	{
		DigJobs.Remove(j.JobCell);
		AssignedDigJobs.Remove(j.JobCell);
	}
	public void RemoveDigJob(BlockController c)
	{
		if(!DigJobs.Remove(c))
		{
			Job j = AssignedDigJobs[c];
			if(j!=null)
			{
				j.CancelJob();
			}
		}
	}

	public bool IsForDig(BlockController c)
	{
		return DigJobs.ContainsKey(c) || AssignedDigJobs.ContainsKey(c);
	}
}
