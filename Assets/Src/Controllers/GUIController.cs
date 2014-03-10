using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, BuildChoose, BuildIngredientsPrepare, BuildIngredients, BuildPlace, Info
	}

	enum ToolButton{
		None, Build,Info
	}
	private Modes state = Modes.Idle;

	public delegate void PickedDelegate(Building pickedBuilding, RecipeInstance recipe);

	public event PickedDelegate ItemPicked;

	public GameObject SelectedObject;

	public GUISkin Skin;

	Building selectedBuilding;
	List<Item> selectedItems = new List<Item>();
	List<Item> itemsCache = new List<Item>();
	public RecipeInstance recipeInstance = new RecipeInstance();
	Action<RecipeInstance> recipeCallback;
	Action recipeCancelCallback;
	int curItem = 0;

	Rect windowRect;
	float panelWidth;
	float mapHeight;
	float toolbarHeight;
	float pad = 10;
	Vector2 buildingScrollPos = new Vector2();
	Vector2 infoWindowScroll = new Vector2();

	// Use this for initialization
	void Start () {
		panelWidth = Screen.width *0.25f;
		mapHeight = Screen.height * 0.25f;
		toolbarHeight = Screen.height*0.1f;

		windowRect=new Rect(Screen.width*0.1f,Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f);
	}

	public bool GetItemsForRecipe(Recipe recipe, Action<RecipeInstance> callback,  Action cancel )
	{
		if(state!=Modes.Idle && state!=Modes.BuildChoose)
			return false;

		selectedItems.Clear();
		curItem = 0;
		recipeCallback=callback;
		recipeCancelCallback = cancel;
		recipeInstance = new RecipeInstance();
		recipeInstance.Prototype = recipe;
		state = Modes.BuildIngredientsPrepare;
		return true;

	}

	void OnGUI()
	{
		ToolButton toolBtn = ToolButton.None;
		GUI.skin = Skin;

		//Map and toolbar box
		GUI.Box(new Rect(0,Screen.height-mapHeight-toolbarHeight,panelWidth,mapHeight+toolbarHeight),"");

		//Left panel rect
		Rect rct = new Rect(0,0,panelWidth,Screen.height-mapHeight-toolbarHeight);

		//Toolbar rect
		Rect toolRect = new Rect(
			0+pad,Screen.height - mapHeight-toolbarHeight+pad,
			panelWidth-pad*2,toolbarHeight-pad*2);


		GUILayout.BeginArea(toolRect);
		GUILayout.BeginHorizontal();

		if(GUILayout.Button("Build"))
			toolBtn = ToolButton.Build;
		
		if(GUILayout.Button("Info"))
			toolBtn = ToolButton.Info;

		GUILayout.EndHorizontal();
		GUILayout.EndArea();



		switch(state)
		{
		case Modes.Idle:
		{

			if(toolBtn==ToolButton.Build)
				state = Modes.BuildChoose;
			else if(toolBtn==ToolButton.Info)
				state = Modes.Info;

			//Left panel box
			GUI.Box(rct,"");
			rct.x+=pad;
			rct.y+=pad;
			rct.width-=pad*2;
			rct.height-=pad*2;
			
			GUILayout.BeginArea(rct);
			
			if (SelectedObject != null)
			{
				Component[] items = SelectedObject.GetComponents<Component>();
				
				foreach(Component item in items)
				{
					if(item is IInteractive)
						((IInteractive)item).OnDrawSelectionGUI();
				}
				
			}
			else
			{
				string date = string.Format("{0,2}.{1,2}.{2,4} {3,2}:{4,2}",M.GameDateTime.Day,
				                            M.GameDateTime.Month,M.GameDateTime.Year,
				                            M.GameDateTime.Hour,M.GameDateTime.Minute);
				GUILayout.Label(date);

				if(GUILayout.Button("Save"))
				{
					M.SaveGame();
				}
				if(GUILayout.Button("Load"))
				{
					M.LoadGame();
				}
				
			}
			GUILayout.EndArea();
		}
			break;
		case Modes.BuildChoose:
		{
			GUI.Box(rct,"");
			rct.x+=pad;
			rct.y+=pad;
			rct.width-=pad*2;
			rct.height-=pad*2;
			GUILayout.BeginArea(rct);
			GUILayout.BeginScrollView(buildingScrollPos);
			
			selectedBuilding = null;
			
			foreach(Building b in M.GameD.Buildings)
			{
				if(b.Hide)
					continue;
				if(GUILayout.Button(b.Name))
				{
					selectedBuilding = b;	
				}
			}
			
			
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			
			if(selectedBuilding!=null)
			{
				

				GetItemsForRecipe(selectedBuilding.recipe,(RecipeInstance res)=>{
					if(ItemPicked!=null)
						ItemPicked(selectedBuilding, res);
					state = Modes.BuildPlace;
				},null);
			}
		}
			break;
		case Modes.BuildPlace:
		{
		}
			break;
		case Modes.BuildIngredientsPrepare:
		case Modes.BuildIngredients:
		{
			GUILayout.Window(0,windowRect,OnDrawIngredientsWindow,"Choose ingredients");
		}
			break;
		case Modes.Info:
		{
			GUILayout.Window(0,windowRect,OnDrawInfoWindow,"Information");
		}
			break;
		}

		
	}

	public void OnDrawIngredientsWindow(int id)
	{
		Ingredient ingredient = recipeInstance.Prototype.IngredientsLinks[curItem];

		if(state == Modes.BuildIngredientsPrepare)
		{
			itemsCache.Clear();



			M.Stat.GetItemsForIngredient(ingredient,itemsCache);

			state = Modes.BuildIngredients;
		}

		Item selected = null;
		GUILayout.Label("Choose item for ingredient #"+(curItem+1));
		GUILayout.BeginScrollView(infoWindowScroll);

		foreach(Item item in itemsCache)
		{
			if(GUILayout.Button(item.Name))
				selected = item;
		}

		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();

		//GUILayout.Button("Build");
		if(GUILayout.Button("Cancel"))
		{
			state = Modes.Idle;
			if(recipeCancelCallback!=null)
				recipeCancelCallback();
		}

		GUILayout.EndHorizontal();

		if(selected !=null)
		{
			recipeInstance.Ingredients.Add(new Pile(selected,ingredient.Quantity));
			curItem++;
			if(curItem>=recipeInstance.Prototype.IngredientsLinks.Count)
			{
				state = Modes.Idle;
				recipeCallback(recipeInstance);

			}
			else
			{
				state = Modes.BuildIngredientsPrepare;
			}
		}


	}

	public void OnDrawInfoWindow(int id)
	{
		infoWindowScroll = GUILayout.BeginScrollView(infoWindowScroll);


		foreach(Item i in M.Stat.Items.Keys)
		{

			GUILayout.BeginHorizontal();
			GUILayout.Label(i.Name);
			GUILayout.Label( (M.Stat.Items[i]/100.0f).ToString("n2") );
			GUILayout.EndHorizontal();

		}
		GUILayout.EndScrollView();
		if(GUILayout.Button("Close"))
			state = Modes.Idle;
	}

	public void OnPlaced()
	{
		if(state==Modes.BuildPlace)
			state= Modes.Idle;
	}

	public void OnDeselect()
	{
		if(state==Modes.Idle)
			SelectedObject = null;
	}
	// Update is called once per frame
	void Update () {
	
	}


}
