using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FurnaceController : BaseManagedController, IInteractive, ICustomer{

	enum Modes {
		Idle, FreeIn,Fill,Prod,FreeOut
	}

	enum SupplyStatus{
		Ready, NotReady, Complete
	}
	Modes state = Modes.Idle;

	public BlockedInventory inInventory, outInventory;

	BuildingController building;

	float productionPoints=0;
	float productionIncome = 0.5f;
	float targetQuantity = 0;
	Recipe targetRecipe;

	Vector2 scroll = new Vector2(0,0);
	List<Recipe> craftableRecipes;
	string[] nameCache;

	List<SupplyJob> supplyJobs = new List<SupplyJob>();

	// Use this for initialization
	void Start () {
		building = GetComponent<BuildingController>();
		if(inInventory==null || outInventory==null)
			throw new UnityException("Inventories must not be null");
		if(inInventory==outInventory)
			throw new UnityException("Inventories must be different");
		inInventory.OnFreed+=OnFreedInput;
		outInventory.OnFreed+=OnFreedOutput;
	}


	public void OnMouseUpAsButton()
	{
		craftableRecipes = M.GameD.RecipesByDevice[building.Prototype.Name];
		nameCache = new string[craftableRecipes.Count];
		int i=0;
		foreach(Recipe r in craftableRecipes)
			nameCache[i++]=r.Name;

		M.GetGUIController().SelectedObject = this;
	}

	// Update is called once per frame
	void Update () {
		SupplyStatus st;
		switch(state)
		{
		case Modes.Fill:
			st = CheckSupply();
			if(st==SupplyStatus.Ready)
			{
				state = Modes.Prod;
				productionPoints=0;
			}
			else if(st==SupplyStatus.Complete)
			{
				state = Modes.FreeOut;
				outInventory.FreeInventory();
			}
			break;
		case Modes.Prod:
			st = CheckSupply();
			if(st==SupplyStatus.NotReady)
			{
				state = Modes.Fill;
			}
			else if(st==SupplyStatus.Complete)
			{
				state = Modes.FreeOut;
				outInventory.FreeInventory();
			}
			else
			{
				productionPoints+=productionIncome*Time.smoothDeltaTime;
				if(productionPoints>1)
				{
					if(!inInventory.TakeForRecipe(targetRecipe))
						state = Modes.Fill;
					else
					{
						productionPoints=0;
						targetQuantity--;
						outInventory.Put(targetRecipe.ResultsLinks[0].ItemType, targetRecipe.ResultsLinks[0].Quantity);
					}
				}
			}
			break;
		}
	}

	void Cancel()
	{
		foreach (SupplyJob sj in supplyJobs)
			sj.Cancel();
		supplyJobs.Clear();

		targetQuantity = 0;
		state = Modes.FreeOut;
		outInventory.FreeInventory();
	}

	void UI()
	{
		if(inInventory.Quantity>0)
		{
			state = Modes.FreeIn;
			inInventory.FreeInventory();
		}
		else
		{
			state = Modes.Fill;
			AddSupplyJobs();
		}
	}

	void OnFreedInput()
	{
		state = Modes.Fill;
		AddSupplyJobs();
	}

	void OnFreedOutput()
	{
		state = Modes.Idle;
		targetRecipe = null;
	}

	void AddSupplyJobs()
	{
		foreach (Pile ingredient in targetRecipe.IngredientsLinks)
		{
			SupplyJob j = new SupplyJob(M.JobManager,this,building,inInventory,
			                            ingredient.ItemType,ingredient.Quantity*targetQuantity);
			M.JobManager.AddJob(j);
			supplyJobs.Add(j);
		}


	}


	SupplyStatus CheckSupply()
	{
		if(targetQuantity<1)
			return SupplyStatus.Complete;

		SupplyStatus res = SupplyStatus.Ready;
		foreach(Pile p in targetRecipe.IngredientsLinks)
		{
			if(inInventory.GetItemQuantity(p.ItemType)<p.Quantity)
			{
				res = SupplyStatus.NotReady;
				break;
			}
		}
		return res;
	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI ()
	{
		if(state == Modes.Idle)
		{
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("+"))
				targetQuantity++;
			GUILayout.Space(5);
			GUILayout.Label(targetQuantity.ToString("n2"));
			GUILayout.Space(5);
			if(GUILayout.Button("-"))
			{
				targetQuantity--;
				if(targetQuantity<0)
					targetQuantity=0;
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("Recipes: ");
			scroll = GUILayout.BeginScrollView(scroll);

			int selected = GUILayout.SelectionGrid(0,nameCache,1);

			GUILayout.Space(20);
			if(GUILayout.Button("Produce"))
			{
				targetRecipe = craftableRecipes[selected];
				UI ();
			}
			GUILayout.EndScrollView();
		}
		else if(state == Modes.Fill || state == Modes.Prod)
		{
			GUILayout.Label("Production: "+targetRecipe.Name);
			if(GUILayout.Button("Cancel"))
				Cancel();
			if(state == Modes.Prod)
			{
				GUILayout.Label("completed: "+productionPoints.ToString("n2"));
			}
		}
		else if(state == Modes.FreeIn)
		{
			GUILayout.Label("Preparing input slot...");
		}
		else if(state == Modes.FreeOut)
		{
			GUILayout.Label("Discharging output...");
		}
	}



	#endregion

	#region ICustomer implementation
	
	public void JobCompleted (IJob j)
	{
		if (state == Modes.Fill || state == Modes.Prod)
		{
			if (j.GetType() != typeof(SupplyJob))
				return;
			SupplyJob sj = (SupplyJob)j;
			supplyJobs.Remove(sj);

			Pile ingredient = targetRecipe.GetIngredient(sj.ItemType);
			float needed = targetQuantity * ingredient.Quantity;
			float have = inInventory.GetItemQuantity(sj.ItemType);
			if (have < needed)
			{
				SupplyJob nj = new SupplyJob(M.JobManager, this, building, inInventory,
			                            ingredient.ItemType, needed - have);
				M.JobManager.AddJob(nj);
				supplyJobs.Add(nj);
			}
		} else
		{
			throw new UnityException("wrong state: "+Enum.GetName(typeof(Modes),state));
		}
	}
	
	#endregion
}
