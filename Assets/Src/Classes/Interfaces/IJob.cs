using System;

public abstract class IJob
{

	protected BlockController blockController;
	protected JobManager jobManager;
	protected IWorker worker;

	public IJob(JobManager jobManager,BlockController block)
	{
		this.blockController=block;
		this.jobManager = jobManager;
	}

	public BlockController BlockController
	{
		get{
			return blockController;
		}
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
		jobManager.CompleteJob(this);
		blockController = null;
		worker = null;
	}
	public void AssignTo(IWorker worker)
	{
		this.worker = worker;
	}

	public abstract void OnDriven();

	public abstract void OnLoaded();

	public abstract void OnUnloaded();

	public abstract void UpdateJob();

	public void Cancel()
	{
		worker.CancelCurrentJob();
		blockController = null;
		worker = null;
	}
}

