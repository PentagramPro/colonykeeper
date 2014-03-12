using System;
using UnityEngine;

public class DigJob : IJob
{
	enum States{
		Start,Go,Dig,Free,Transport,End
	}
	BlockController blockController;
	States state = States.Start;


	public DigJob()
	{
	}
	public DigJob (JobManager jobManager, ICustomer customer,BlockController block) 
		: base(jobManager,customer)
	{
		blockController = block;
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

		Complete();
		state = States.End;
	}
	void HandleDig()
	{
		switch(worker.Dig(blockController))
		{
		case BlockController.DigResult.DestinationFull:
			state = States.Free;
			worker.Unload();

			break;
		case BlockController.DigResult.Finished:
			End();
			break;
		case BlockController.DigResult.NotFinished:
			break;
		}
	}

	public override void Save (WriterEx b)
	{
		base.Save (b);
		b.WriteEnum(state);
		b.Write(blockController.GetUID());
	}

	public override void Load (Manager m, ReaderEx r)
	{
		base.Load (m, r);
		state = (States)r.ReadEnum(typeof(States));
		blockController = (BlockController)m.LoadedLinks[r.ReadInt32()];
	}


}


