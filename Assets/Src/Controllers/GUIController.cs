using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, BuildChoose, BuildIngredients, BuildPlace, Info
	}

	enum ToolButton{
		None, Build,Info
	}
	private Modes state = Modes.Idle;

	public delegate void PickedDelegate(Building pickedBuilding, RecipeInstance recipe);

	public event PickedDelegate ItemPicked;

	public GameObject SelectedObject{
		set{
			leftPanelWnd.SelectedObject = value;
		}
	}



	public WindowController WC;

	ChooseItemsWindow chooseItemsWnd;
	LeftPanelWindow leftPanelWnd;
	BuildingsWindow buildingsWnd;

	Building selectedBuilding;

	public RecipeInstance LastRecipeInstance{
		get{
			return chooseItemsWnd.recipeInstance;
		}
	}


	Rect windowRect;
	float panelWidth;
	float mapHeight;




	// Use this for initialization
	void Start () {
		panelWidth = Screen.width *0.25f;
		mapHeight = Screen.height * 0.25f;


		Rect leftRect = new Rect(0,0,panelWidth,Screen.height - mapHeight);

		windowRect=new Rect(Screen.width*0.1f,Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f);
		chooseItemsWnd = new ChooseItemsWindow(windowRect,OnItemsChoose);
		leftPanelWnd = new LeftPanelWindow(leftRect,OnToolBuild,OnToolInfo);
		buildingsWnd = new BuildingsWindow(leftRect,OnBuildingsChoose);

		WC.AddWindow(leftPanelWnd);
		WC.AddWindow(buildingsWnd);

		buildingsWnd.Show = false;

	}

	void OnToolBuild()
	{
		if(state==Modes.Idle)
		{
			leftPanelWnd.Show = false;
			buildingsWnd.Show = true;
		}
	}

	void OnToolInfo()
	{

	}

	void OnBuildingsChoose(Building building)
	{
		leftPanelWnd.Show = false;
		buildingsWnd.Show = false;

		GetItemsForRecipe(building.recipe,(RecipeInstance res)=>{
			if(ItemPicked!=null)
				ItemPicked(building, res);
			state = Modes.BuildPlace;
		},null);
	}

	void OnItemsChoose(KWindow.Results results)
	{

	}

	public bool GetItemsForRecipe(Recipe recipe, Action<RecipeInstance> callback,  Action cancel )
	{
		if(state!=Modes.Idle)
			return false;

		chooseItemsWnd.recipeCallback=callback;
		chooseItemsWnd.recipeCancelCallback = cancel;


		chooseItemsWnd.recipeInstance = new RecipeInstance();
		chooseItemsWnd.recipeInstance.Prototype = recipe;
		state = Modes.BuildIngredients;
		WC.AddWindow(chooseItemsWnd);


		return true;

	}

	void OnGUI()
	{



		//Map  box
		GUI.Box(new Rect(0,Screen.height-mapHeight,panelWidth,mapHeight),"");


/*


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
		case Modes.BuildIngredients:
		{
			KWindow.Results result = chooseItemsWnd.Draw();
			if(result==KWindow.Results.Ok || result==KWindow.Results.Close)
				state= Modes.Idle;
			
		}
			break;
		case Modes.Info:
		{
			GUILayout.Window(0,windowRect,OnDrawInfoWindow,"Information");
		}
			break;
		}

		*/
	}
	/*

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
	}*/

	public void OnPlaced()
	{
		if(state==Modes.BuildPlace)
		{
			leftPanelWnd.Show=true;
			state= Modes.Idle;
		}
	}

	public void OnDeselect()
	{
		if(state==Modes.Idle)
			leftPanelWnd.SelectedObject = null;
	}
	// Update is called once per frame
	void Update () {
	
	}


}
