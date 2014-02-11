using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FurnaceController : BaseManagedController, IInteractive{

	enum Modes {
		Idle, FreeIn,Fill,Prod,FreeOut
	}


	Modes state = Modes.Idle;

	public BlockedInventory inInventory, outInventory;
	public SupplyController supplyController;

	BuildingController building;

	float productionPoints=0;
	float productionIncome = 0.5f;
	int targetQuantity = 0;
	Recipe targetRecipe;

	Vector2 scroll = new Vector2(0,0);
	List<Recipe> craftableRecipes;
	string[] nameCache;


	// Use this for initialization
	void Start () {
		building = GetComponent<BuildingController>();
		if(inInventory==null || outInventory==null)
			throw new UnityException("Inventories must not be null");
		if(inInventory==outInventory)
			throw new UnityException("Inventories must be different");
		if(supplyController==null)
			throw new UnityException("Supply controller must not be null");
		inInventory.OnFreed+=OnFreedInput;
		outInventory.OnFreed+=OnFreedOutput;
	}




	// Update is called once per frame
	void Update () {
		SupplyController.SupplyStatus st;
		switch(state)
		{
		case Modes.Fill:
			st = supplyController.CheckSupply(targetRecipe,targetQuantity);
			if(st==SupplyController.SupplyStatus.Ready)
			{
				state = Modes.Prod;
				productionPoints=0;
			}
			else if(st==SupplyController.SupplyStatus.Complete)
			{
				state = Modes.FreeOut;
				outInventory.FreeInventory();
			}
			break;
		case Modes.Prod:
			st = supplyController.CheckSupply(targetRecipe,targetQuantity);
			if(st==SupplyController.SupplyStatus.NotReady)
			{
				state = Modes.Fill;
			}
			else if(st==SupplyController.SupplyStatus.Complete)
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
		supplyController.Cancel();
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
		supplyController.Supply(targetRecipe,targetQuantity);
	}




	#region IInteractive implementation

	public void OnDrawSelectionGUI ()
	{
		if (craftableRecipes == null || nameCache==null)
		{
			craftableRecipes = M.GameD.RecipesByDevice[building.Prototype.Name];
			nameCache = new string[craftableRecipes.Count];
			int i=0;
			foreach(Recipe r in craftableRecipes)
				nameCache[i++]=r.Name;
		}
		if(state == Modes.Idle)
		{
			GUILayout.BeginHorizontal();

			if(GUILayout.Button("-"))
			{
				targetQuantity--;
				if(targetQuantity<0)
					targetQuantity=0;
			}
			GUILayout.Space(5);
			GUILayout.Label(targetQuantity.ToString("n2"));
			GUILayout.Space(5);

			if(GUILayout.Button("+"))
				targetQuantity++;

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


}
