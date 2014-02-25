using UnityEngine;
using System.Collections.Generic;
using System;

public class SupplyController : BaseManagedController, ICustomer, IStorable {

	enum Modes{
		Idle, Supply
	}

	public enum SupplyStatus{
		Ready, NotReady, Complete
	}
	public IInventory InInventory;

	Modes state= Modes.Idle;
	BuildingController building;
	RecipeInstance targetRecipe;
	int targetQuantity;
	List<SupplyJob> supplyJobs = new List<SupplyJob>();

	public void Supply(RecipeInstance recipe, int quantity)
	{

		this.targetRecipe = recipe;
		this.targetQuantity = quantity;


		foreach (Pile ingredient in targetRecipe.Ingredients)
		{
			SupplyJob j = new SupplyJob(M.JobManager,this,building,InInventory,
			                            ingredient.ItemType,ingredient.Quantity*targetQuantity);
			M.JobManager.AddJob(j,false);
			supplyJobs.Add(j);
		}
		state = Modes.Supply;
	}

	public void Cancel()
	{
		if(state == Modes.Supply)
		{
			foreach (SupplyJob sj in supplyJobs)
				sj.Cancel();
			supplyJobs.Clear();
			
			targetQuantity = 0;
		}
		state=Modes.Idle;
	}

	// may be called several times 
	public void Init()
	{
		if (building == null)
		{
			building = GetComponent<BuildingController>();
			if (building == null)
				throw new UnityException("Cannot find BuildingController");
		}
	}
	// Use this for initialization
	void Start () {
		if(InInventory==null)
			throw new UnityException("Inventory must not be null");
		Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public SupplyStatus CheckSupply(RecipeInstance targetRecipe, int targetQuantity)
	{
		if(targetQuantity<1)
			return SupplyStatus.Complete;
		
		SupplyStatus res = SupplyStatus.Ready;
		foreach(Pile p in targetRecipe.Ingredients)
		{
			if(InInventory.GetItemQuantity(p.ItemType)<p.Quantity)
			{
				res = SupplyStatus.NotReady;
				break;
			}
		}
		return res;
	}
	#region ICustomer implementation


	public void JobCompleted (IJob job)
	{
		if (state == Modes.Supply)
		{
			if (job.GetType() != typeof(SupplyJob))
				return;
			SupplyJob sj = (SupplyJob)job;
			supplyJobs.Remove(sj);
			
			Item itemType = sj.ItemType;
			int needed = targetQuantity * targetRecipe.GetIngredient(sj.ItemType);
			int have = InInventory.GetItemQuantity(sj.ItemType);
			if (have < needed)
			{
				SupplyJob nj = new SupplyJob(M.JobManager, this, building, InInventory,
				                             itemType, needed - have);
				M.JobManager.AddJob(nj,false);
				supplyJobs.Add(nj);
			}

			if(supplyJobs.Count==0)
				state = Modes.Idle;
		} else
		{
			throw new UnityException("wrong state: "+Enum.GetName(typeof(Modes),state));
		}
	}


	#endregion

	#region IStorable implementation
	public void Save (WriterEx b)
	{
		b.WriteEnum(state);

		b.WriteLink(building);
		b.WriteLink(targetRecipe);
		b.Write(targetQuantity);

		b.Write(supplyJobs.Count);
		foreach(SupplyJob s in supplyJobs)
			b.WriteLink(s);

	}
	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));

		building = (BuildingController)r.ReadLink(m);
		targetRecipe = (RecipeInstance) r.ReadLink(m);
		targetQuantity = r.ReadInt32();

		supplyJobs.Clear();
		int count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			supplyJobs.Add((SupplyJob)r.ReadLink(m));
		}
	}
	#endregion
}
