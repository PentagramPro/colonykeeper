using System;

public abstract class IJob
{


	protected JobManager jobManager;
	protected IWorker worker;
	protected ICustomer customer;
	public IJob(JobManager jobManager, ICustomer customer)
	{
		this.customer = customer;
		this.jobManager = jobManager;
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
}

