using UnityEngine;
using System.Collections;

public class UnloadController : BaseManagedController,ICustomer, IStorable {

	public IInventory InventoryToUnload;
	enum Modes {
		Idle, Unload
	}
	Modes state = Modes.Idle;

	public delegate void InventoryEvent();

	public event InventoryEvent OnFreed;
	
	public void FreeInventory()
	{
		if(InventoryToUnload.Quantity==0)
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

		UnloadJob j = new UnloadJob(M.JobManager,this,bc,InventoryToUnload);
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
			if(InventoryToUnload.GetItemQuantity(type)>=p.Quantity)
				InventoryToUnload.Take(type,p.Quantity);
			else
				return false;
		}
		
		return true;
	}
	#region ICustomer implementation


	void CheckJob()
	{
		if(state == Modes.Unload)
		{
			if(InventoryToUnload.Quantity>0)
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
	public void JobCanceled(IJob j)
	{
		CheckJob();
	}

	public void JobCompleted (IJob j)
	{
		CheckJob();
	}
	#endregion		
	

	
	public void Save (WriterEx b)
	{

		b.WriteEnum(state);
	}
	
	public void Load (Manager m, ReaderEx r)
	{

		state = (Modes)r.ReadEnum(typeof(Modes));
	}
}
