using UnityEngine;
using System.Collections.Generic;

public class JobManager  {


	public delegate void JobNotification(Job j);
	public event JobNotification DigJobAdded;

	Dictionary<Cell,Job> DigJobs = new Dictionary<Cell, Job>();
	Dictionary<Cell,Job> AssignedDigJobs = new Dictionary<Cell, Job>();

	public void AddDigJob(Cell c)
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
	public void RemoveDigJob(Cell c)
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

	public bool IsForDig(Cell c)
	{
		return DigJobs.ContainsKey(c);
	}
}
