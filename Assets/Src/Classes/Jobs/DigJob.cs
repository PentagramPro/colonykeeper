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
			state= States.Go;
		else if(state==States.Transport)
			End ();
		else
			throw new UnityException("Wrong state!");
	}
	#endregion

	void End()
	{
		Complete();
		state = States.End;
	}
	void HandleDig()
	{
		switch(worker.Dig())
		{
		case BlockController.DigResult.DestinationFull:
			worker.Unload();
			state = States.Free;
			break;
		case BlockController.DigResult.Finished:
			worker.Unload();
			state = States.Transport;
			break;
		case BlockController.DigResult.NotFinished:
			break;
		}
	}
}


