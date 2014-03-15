using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, BuildChoose, BuildIngredients, BuildPlace, Info
	}
	float mapHeight;
	float panelWidth;
	private Modes state = Modes.Idle;

	public delegate void PickedDelegate(Building pickedBuilding, RecipeInstance recipe);

	public event PickedDelegate ItemPicked;


	public WindowController WC;

	ChooseItemsWindow chooseItemsWnd;
	LeftPanelWindow leftPanelWnd;
	BuildingsWindow buildingsWnd;
	LogWindow logWnd;

	public GameObject SelectedObject{
		set{
			leftPanelWnd.SelectedObject = value;
		}
	}

	public RecipeInstance LastRecipeInstance{
		get{
			return chooseItemsWnd.recipeInstance;
		}
	}


	// Use this for initialization
	void Start () {
		panelWidth = Screen.width *0.25f;
		mapHeight = Screen.height * 0.25f;
		
		Rect leftRect = new Rect(0,0,panelWidth,Screen.height - mapHeight);
		Rect windowRect=new Rect(Screen.width*0.1f,Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f);

		chooseItemsWnd = new ChooseItemsWindow(windowRect,OnItemsChoose);
		leftPanelWnd = new LeftPanelWindow(leftRect,OnToolBuild,OnToolInfo);
		buildingsWnd = new BuildingsWindow(leftRect,OnBuildingsChoose);
		logWnd = new LogWindow(new Rect(panelWidth,Screen.height-50,Screen.width-panelWidth,50));

		WC.AddWindow(leftPanelWnd);
		WC.AddWindow(buildingsWnd);
		WC.AddWindow(logWnd);

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
