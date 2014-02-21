using UnityEngine;
using System.Collections.Generic;

public class JobManager : IStorable {


	public delegate void JobNotification(IJob j);
	public event JobNotification JobAdded;

	List<IJob> Jobs = new List<IJob>();
	List<IJob> BlockedJobs = new List<IJob>();
	List<IJob> AssignedJobs = new List<IJob>();
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
			AssignedJobs.Add(j);
			return true;
		}
		return false;
	}


	public void CompleteJob(IJob j)
	{
		AssignedJobs.Remove(j);
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

		AssignedJobs.Remove(j);
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
		b.Write(Jobs.Count);
		foreach(IJob j in Jobs)
			j.SaveUid(b);
		b.Write(BlockedJobs.Count);
		foreach(IJob j in BlockedJobs)
			j.SaveUid(b);

		b.Write(AssignedJobs.Count);
		foreach(IJob j in AssignedJobs)
			j.SaveUid(b);

		b.Write(DelayedJobs.Count);
		foreach(float key in DelayedJobs.Keys)
		{
			b.Write(key);
			DelayedJobs[key].SaveUid(b);
		}
			
	}
	
	public void LoadUid(Manager m, ReaderEx r)
	{

		Jobs.Clear();
		BlockedJobs.Clear();
		int count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			IJob j = IJob.LoadFactory(m,r);
			j.LoadUid(m,r);
			Jobs.Add(j);
		}

		count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			IJob j = IJob.LoadFactory(m,r);
			j.LoadUid(m,r);
			AssignedJobs.Add(j);
		}


		count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			IJob j = IJob.LoadFactory(m,r);
			j.LoadUid(m,r);
			AssignedJobs.Add(j);
		}

		count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			float key = (float)r.ReadDouble();
			IJob j = IJob.LoadFactory(m,r);
			j.LoadUid(m,r);
			DelayedJobs.Add(key,j);
		}
	}
	
	public void Save (WriterEx b)
	{
		foreach(IJob j in Jobs)
			j.Save(b);
		foreach(IJob j in BlockedJobs)
			j.Save(b);
		foreach(IJob j in AssignedJobs)
			j.Save(b);
		foreach(IJob j in DelayedJobs.Values)
			j.Save(b);

	}
	public void Load (Manager m, ReaderEx r)
	{
		foreach(IJob j in Jobs)
			j.Load(m,r);
		foreach(IJob j in BlockedJobs)
			j.Load(m,r);
		foreach(IJob j in AssignedJobs)
			j.Load(m,r);
		foreach(IJob j in  DelayedJobs.Values)
			j.Load(m,r);
	}

	public int GetUID()
	{
		return 0;
	}
	#endregion
}
