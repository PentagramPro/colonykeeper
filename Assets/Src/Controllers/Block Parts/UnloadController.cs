using UnityEngine;
using System.Collections;

public class UnloadController : BaseManagedController,ICustomer, IStorable {

	public enum OperaionModes{
		OutputItems,OutputVehicles
	}
	public IInventory InventoryToUnload;
	enum Modes {
		Idle, Unload
	}
	Modes state = Modes.Idle;
	public OperaionModes OperationMode = OperaionModes.OutputItems;

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


	public void PutProduction(RecipeInstance r)
	{
		switch(OperationMode)
		{
			case OperaionModes.OutputItems:
			{
				Pile res = new Pile(r.ResultsLinks[0].ItemType,r.ResultsLinks[0].Quantity);
				int index=0;
				foreach(Ingredient i in r.Prototype.Ingredients)
				{
					Pile thatPile = r.Ingredients[index];
					foreach(string sp in i.Properties)
					{
						if(sp=="Color")
							res.Properties.color = thatPile.Properties.color;
						else if (sp=="Color2")
							res.Properties.secondaryColor = thatPile.Properties.secondaryColor;
						else
							res.Properties[sp]=thatPile.Properties[sp];
					}
					index++;
				}
				//res.Properties = r.Ingredients[0].Properties.copy();
				InventoryToUnload
					.Put(res);
			}
			break;
			case OperaionModes.OutputVehicles:
			{
				VehicleController res =  M.CreateVehicle(r.Prototype.vehicle.Name,transform.position);
			}
			break;
		}
	}
	void AddJob()
	{
		BuildingController bc = GetComponent<BuildingController>();
		Vector3 pos;
		if (bc == null)
			pos = transform.position;
		else
			pos = bc.Position;

		UnloadJob j = new UnloadJob(M.JobManager,this,pos,InventoryToUnload);
		M.JobManager.AddJob(j,false);
	}
	
	void CallOnFreed()
	{
		if(OnFreed!=null)
			OnFreed();
	}
	
	// take items for recipe. Items are disappeared because they are considered to be 'used' for production
	public bool TakeForRecipe(RecipeInstance r)
	{
		foreach(Pile p in r.Ingredients)
		{
			if(!InventoryToUnload.CanTake(new PileRequest(p,p.Quantity),true))
				return false;
		}

		foreach(Pile p in r.Ingredients)
			InventoryToUnload.Take(new PileRequest(p,p.Quantity));
		
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
