using System;
using System.Collections.Generic;
public abstract class IJob : IStorable
{

	UidContainer uidc;

	protected JobManager jobManager;
	protected IWorker worker;

	protected WeakReference customer;
	protected ICustomer Customer{
		get{
			if(customer==null)
				return null;

			return customer.Target as ICustomer;
		}
		set{
			if(value == null)
				customer = null;
			else
				customer = new WeakReference(value);
		}
	}

	public IJob()
	{
		uidc = new UidContainer(this);
	}
	public IJob(JobManager jobManager, ICustomer customer)
	{
		Customer = customer;
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
		if(Customer!=null)
			Customer.JobCompleted(this);
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
		if(worker!=null)
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
		b.WriteLink((IStorable)Customer);
	}

	public virtual void Load (Manager m, ReaderEx r)
	{
		jobManager = m.JobManager;

		worker = (IWorker)r.ReadLink(m);
		Customer = (ICustomer)r.ReadLink(m);
	}

	#endregion
}

