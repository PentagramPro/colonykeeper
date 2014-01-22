using System;
using UnityEngine;

public class DigJob : IJob
{
	enum States{
		Start,Go,Dig,Free,Transport,End
	}
	States state = States.Start;

	public DigJob (JobManager jobManager,BlockController block) 
		: base(jobManager,block)
	{
	}
	
	#region IJob implementation
	public override void UpdateJob()
	{
		switch(state)
		{
		case States.Start:
			worker.DriveTo(blockController.Position);
			state = States.Go;
			break;
		case States.Go:
			break;
		case States.Dig:
			HandleDig();
			break;
		case States.Free:
			break;
		case States.Transport:
			break;

		}
	}

	public override void OnDriven ()
	{
		if(state==States.Go)
		{
			state = States.Dig;
		}
		else
			throw new UnityException("Wrong state!");
	}

	public override void OnLoaded ()
	{
		throw new UnityException("Wrong state!");
	}

	public override void OnUnloaded ()
	{
		if(state==States.Free)
			state= States.Start;
		else if(state==States.Transport)
			End ();
		else
			throw new UnityException("Wrong state!");
	}
	#endregion

	void End()
	{
		blockController.JobCompleted();
		Complete();
		state = States.End;
	}
	void HandleDig()
	{
		switch(worker.Dig())
		{
		case BlockController.DigResult.DestinationFull:
			state = States.Free;
			worker.Unload();

			break;
		case BlockController.DigResult.Finished:
			state = States.Transport;
			worker.Unload();

			break;
		case BlockController.DigResult.NotFinished:
			break;
		}
	}
}


