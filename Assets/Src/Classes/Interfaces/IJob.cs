using System;

public abstract class IJob : IStorable
{

	UidContainer uidc;

	protected JobManager jobManager;
	protected IWorker worker;
	protected ICustomer customer;

	public IJob()
	{
		uidc = new UidContainer(this);
	}
	public IJob(JobManager jobManager, ICustomer customer)
	{
		this.customer = customer;
		this.jobManager = jobManager;
		uidc = new UidContainer(this);
	}



	public IWorker Worker
	{
		get{
			return worker;
		}
	}

	protected void Complete()
	{
		worker.OnJobCompleted();
		customer.JobCompleted(this);
		jobManager.CompleteJob(this);
		customer = null;
		worker = null;
	}
	public void AssignTo(IWorker worker)
	{
		this.worker = worker;
	}

	protected void DelayThisJob()
	{
		worker.CancelCurrentJob();

		worker = null;
		jobManager.DelayJob(this);
	}

	public abstract void OnDriven();

	public abstract void OnLoaded();

	public abstract void OnUnloaded();

	public abstract void UpdateJob();

	public void Cancel()
	{
		worker.CancelCurrentJob();
		customer = null;
		worker = null;
	}

	public int GetUID()
	{
		return uidc.UID;
	}
	public static IJob LoadFactory(Manager m, ReaderEx r)
	{
		var type = Type.GetType(r.ReadString());
		return (IJob)Activator.CreateInstance(type);
	}
	#region IStorable implementation

	public void SaveUid(WriterEx b)
	{
		b.Write(GetType().FullName);
		uidc.Save(b);
	}
	
	public void LoadUid(Manager m, ReaderEx r)
	{
		uidc.Load(m,r);
	}

	public virtual void Save (WriterEx b)
	{
		b.WriteLink((IStorable)worker);
		b.WriteLink((IStorable)customer);
	}

	public virtual void Load (Manager m, ReaderEx r)
	{
		jobManager = m.JobManager;

		worker = (IWorker)r.ReadLink(m);
		customer = (ICustomer)r.ReadLink(m);
	}

	#endregion
}

