using UnityEngine;
using System.Collections.Generic;

public class JobManager  {
	List<Job> Jobs = new List<Job>();

	Dictionary<Cell,Job> DigJobs = new Dictionary<Cell, Job>();

	public void AddDigJob(Cell c)
	{
		Job j = new Job();
		j.JobCell=c;
		DigJobs.Add(c,j);
	}

	public void RemoveDigJob(Cell c)
	{
		DigJobs.Remove(c);
	}

	public bool IsForDig(Cell c)
	{
		return DigJobs.ContainsKey(c);
	}
}
