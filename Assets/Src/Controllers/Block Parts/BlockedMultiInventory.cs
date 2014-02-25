using UnityEngine;
using System.Collections;

public class BlockedMultiInventory : MultiInventory, ICustomer, IStorable {

	enum Modes {
		Idle, Unload
	}
	Modes state = Modes.Idle;
	
	public event InventoryEvent OnFreed;
	
	public void FreeInventory()
	{
		if(Quantity==0)
		{
			CallOnFreed();
		}
		else
		{
			state = Modes.Unload;
			AddJob();
		}
	}
	
	void AddJob()
	{
		BuildingController bc = GetComponent<BuildingController>();
		if(bc == null)
			throw new UnityException("BlockedInventory should be attached to GameObject with BuildingController attached");
		UnloadJob j = new UnloadJob(M.JobManager,this,bc,this);
		M.JobManager.AddJob(j,false);
	}
	
	void CallOnFreed()
	{
		if(OnFreed!=null)
			OnFreed();
	}
	
	
	public bool TakeForRecipe(RecipeInstance r)
	{
		foreach(Pile p in r.Ingredients)
		{
			Item type = p.ItemType;
			if(GetItemQuantity(type)>=p.Quantity)
				Take(type,p.Quantity);
			else
				return false;
		}
		
		return true;
	}
	#region ICustomer implementation
	public void JobCompleted (IJob j)
	{
		if(state == Modes.Unload)
		{
			if(Quantity>0)
			{
				AddJob();
			}
			else
			{
				state = Modes.Idle;
				CallOnFreed();
			}
		}
	}
	#endregion		
	
	public override int CanPut (Item item)
	{
		return 0;
	}
	
	public override bool CanTake (Item item)
	{
		return false;
	}

	public override void Save (WriterEx b)
	{
		base.Save (b);
		b.WriteEnum(state);
	}

	public override void Load (Manager m, ReaderEx r)
	{
		base.Load (m, r);
		state = (Modes)r.ReadEnum(typeof(Modes));
	}
}
