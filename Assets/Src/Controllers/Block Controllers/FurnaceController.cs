﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FurnaceController : BaseManagedController, IInteractive, IStorable{

	enum Modes {
		Idle, Ingredients, FreeIn,Fill,Prod,FreeOut
	}


	Modes state = Modes.Idle;
	float productionPoints=0;
	int targetQuantity = 0;
	RecipeInstance targetRecipe;



	public SupplyController supplyController;
	public UnloadController inputUnloadController, outputUnloadController;

	BuildingController building;

	int selectedItem=0;
	float productionIncome = 0.5f;

	Vector2 scroll = new Vector2(0,0);
	List<Recipe> craftableRecipes;
	string[] nameCache;


	// Use this for initialization
	void Start () {
		building = GetComponent<BuildingController>();


		if(inputUnloadController==null || outputUnloadController==null)
			throw new UnityException("Unload controllers must not be null");
		if(inputUnloadController==outputUnloadController)
			throw new UnityException("Unload controllers must be different");


		if(supplyController==null)
			throw new UnityException("Supply controller must not be null");
		inputUnloadController.OnFreed+=OnFreedInput;
		outputUnloadController.OnFreed+=OnFreedOutput;
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
				outputUnloadController.FreeInventory();
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
				outputUnloadController.FreeInventory();
			}
			else
			{
				productionPoints+=productionIncome*Time.smoothDeltaTime;
				if(productionPoints>1)
				{
					productionPoints=0;
					if(!inputUnloadController.TakeForRecipe(targetRecipe))
						state = Modes.Fill;
					else
					{

						targetQuantity--;
						outputUnloadController.InventoryToUnload.Put(targetRecipe.ResultsLinks[0].ItemType, targetRecipe.ResultsLinks[0].Quantity);
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
		outputUnloadController.FreeInventory();

	}

	void UI()
	{
		if(inputUnloadController.InventoryToUnload.Quantity>0)
		{
			state = Modes.FreeIn;
			inputUnloadController.FreeInventory();
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

			selectedItem = GUILayout.SelectionGrid(selectedItem,nameCache,1);

			GUILayout.Space(20);
			if(GUILayout.Button("Produce"))
			{

				Recipe recipe =craftableRecipes[selectedItem];
				//---//
				if(recipe.IsOneCombination)
				{
					targetRecipe = new RecipeInstance();
					targetRecipe.Prototype = recipe;
					foreach(Ingredient ing in recipe.IngredientsLinks)
						targetRecipe.Ingredients.Add(new Pile(ing.Items[0],ing.Quantity));
					UI ();
				}
				else
				{
					if(M.GetGUIController().GetItemsForRecipe(recipe,(RecipeInstance res)=>{
						targetRecipe = res;
						UI ();
					},()=>{
						state = Modes.Idle;
					}))
					{
						state = Modes.Ingredients;
					}
				}
				//---//

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


	#region IStorable implementation

	public override void SaveUid (WriterEx b)
	{
		base.SaveUid (b);

		if(targetRecipe!=null)
		{
			b.Write(true);
			targetRecipe.SaveUid(b);
		}
		else
			b.Write(false);

	}

	public override void LoadUid (Manager m, ReaderEx r)
	{
		base.LoadUid (m, r);
		targetRecipe = null;
		if(r.ReadBoolean())
		{
			targetRecipe = new RecipeInstance();
			targetRecipe.LoadUid(m,r);
		}
	}

	public void Save (WriterEx b)
	{
		b.WriteEnum(state);
		b.Write((double)productionPoints);
		b.Write(targetQuantity);
			targetRecipe.Save(b);
		

	
	}
	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		productionPoints = (float)r.ReadDouble();
		targetQuantity = r.ReadInt32();


		if(targetRecipe!=null)
			targetRecipe.Load(m,r);


	}
	#endregion
}
