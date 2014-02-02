using UnityEngine;
using System.Collections;

public class SupplyController : BaseManagedController, ICustomer {

	enum Modes{
		Idle, Supply
	}

	public BlockedInventory InInventory;

	Modes state= Modes.Idle;
	BuildingController building;
	Recipe targetRecipe;
	int targetQuantity;
	List<SupplyJob> supplyJobs = new List<SupplyJob>();

	public void Supply(Recipe recipe, int quantity)
	{
		this.targetRecipe = recipe;
		this.targetQuantity = quantity;


		foreach (Pile ingredient in targetRecipe.IngredientsLinks)
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


	// Use this for initialization
	void Start () {
		if(InInventory==null)
			throw new UnityException("Inventory must not be null");
		building = GetComponent<BuildingController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region ICustomer implementation


	public void JobCompleted (IJob job)
	{
		if (state == Modes.Supply)
		{
			if (j.GetType() != typeof(SupplyJob))
				return;
			SupplyJob sj = (SupplyJob)j;
			supplyJobs.Remove(sj);
			
			Pile ingredient = targetRecipe.GetIngredient(sj.ItemType);
			int needed = targetQuantity * ingredient.Quantity;
			int have = inInventory.GetItemQuantity(sj.ItemType);
			if (have < needed)
			{
				SupplyJob nj = new SupplyJob(M.JobManager, this, building, inInventory,
				                             ingredient.ItemType, needed - have);
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

}
