//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.17929
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

public class UnloadJob : IJob
{
	enum Modes
	{
		Start, Go,Pick,Unload,End
	}

	Modes state = Modes.Start;
	IInventory inventory;

	public UnloadJob(JobManager jobManager,BlockController target, IInventory targetInventory) 
		: base(jobManager,target)
	{
		inventory = targetInventory;
	}

	#region implemented abstract members of IJob

	public override void OnDriven ()
	{
		Item[] items = inventory.GetItemTypes();
		if(items.GetLength(0)>0)
		{
			float q = inventory.GetItemQuantity(items[0]);
			worker.Pick(inventory,items[0],q);
			state = Modes.Unload;
			worker.Unload();
		}
		else
		{
			state=Modes.End;
			Complete();
		}

	}

	public override void OnLoaded ()
	{

	}

	public override void OnUnloaded ()
	{
		state=Modes.End;
		Complete();
	}

	public override void UpdateJob ()
	{
		switch(state)
		{
		case Modes.Start:
			worker.DriveTo(BlockController.Position);
			state = Modes.Go;
			break;
		}
	}

	#endregion
}

