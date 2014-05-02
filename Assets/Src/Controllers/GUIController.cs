using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, BuildChoose, BuildIngredients, BuildPlace, Info, Ingredients
	}
	float mapHeight;
	float panelWidth;
	private Modes _state = Modes.Idle;

	private Modes state{
		get{
			return _state;
		}
		set{
			_state = value;
			LF.Info("GUI state is "+Enum.GetName(typeof(Modes),value));
		}
	}

	public delegate void PickedDelegate(Building pickedBuilding, RecipeInstance recipe);

	public event PickedDelegate ItemPicked;


	public WindowController WC;

	ChooseItemsWindow chooseItemsWnd;
	LeftPanelWindow leftPanelWnd;
	BuildingsWindow buildingsWnd;
	InfoWindow infoWnd;
	LogWindow logWnd;

	public GameObject SelectedObject{
		set{
			if(value!=null)
				LF.Info("SelectedObject is set to "+value.name);
			if(leftPanelWnd.SelectedObject!=null)
				CallOnDeselected(leftPanelWnd.SelectedObject);
			leftPanelWnd.SelectedObject = value;
			if(value!=null)
				CallOnSelected(value);
		}
	}

	public RecipeInstance LastRecipeInstance{
		get{
			return chooseItemsWnd.recipeInstance;
		}
	}


	// Use this for initialization
	void Start () {
		LF.Info("============ GUI Start procedure ============");
		panelWidth = WC.NWidth *0.25f;
		mapHeight = WC.NHeight * 0.25f;
		
		Rect leftRect = new Rect(0,0,panelWidth,WC.NHeight - mapHeight);
		Rect windowRect=new Rect(WC.NWidth*0.1f,WC.NHeight*0.1f, WC.NWidth*0.8f, WC.NHeight*0.8f);

		chooseItemsWnd = new ChooseItemsWindow(windowRect,OnItemsChoose);
		leftPanelWnd = new LeftPanelWindow(leftRect,OnToolBuild,OnToolInfo);
		buildingsWnd = new BuildingsWindow(leftRect,OnBuildingsChoose);
		logWnd = new LogWindow(new Rect(panelWidth,WC.NHeight-50,WC.NWidth-panelWidth,50), // Collapsed rect
		                       new Rect(panelWidth,WC.NHeight-400,WC.NWidth-panelWidth,400) // Expanded rect
		                       );
		infoWnd = new InfoWindow(windowRect, OnInfoResult);

		WC.AddWindow(leftPanelWnd);
		WC.AddWindow(buildingsWnd);
		WC.AddWindow(logWnd);

		buildingsWnd.Show = false;

	}

	public void DisplayMessage(string message, Vector3 pos, Color color)
	{
		logWnd.DisplayMessage(message,pos,color);
	}

	void OnToolBuild()
	{
		if(state==Modes.Idle)
		{
			LF.Info("OnToolBuild");
			leftPanelWnd.Show = false;
			buildingsWnd.Show = true;
		}
	}

	void OnToolInfo()
	{
		WC.AddWindow(infoWnd);
		LF.Info("OnToolInfo");
	}

	void OnInfoResult(KWindow.Results res)
	{

	}

	void OnBuildingsChoose(Building building)
	{
		leftPanelWnd.Show = false;
		buildingsWnd.Show = false;

		LF.Info("OnBuildingsChoose");
		state = Modes.BuildIngredients;

		GetItemsForRecipe(building.recipe,(RecipeInstance res)=>{
			if(ItemPicked!=null)
				ItemPicked(building, res);
			state = Modes.BuildPlace;
		},()=>{

		});
	}

	void OnItemsChoose(KWindow.Results results)
	{
		if(results==KWindow.Results.Close || state == Modes.Ingredients)
		{
			state = Modes.Idle;
			leftPanelWnd.Show = true;
			buildingsWnd.Show = false;
		}
	}

	public bool GetItemsForRecipe(Recipe recipe, Action<RecipeInstance> callback,  Action cancel )
	{
		if(state==Modes.Idle)
			state = Modes.Ingredients;
		else if(state!=Modes.BuildIngredients)
			return false;

		chooseItemsWnd.recipeCallback=callback;
		chooseItemsWnd.recipeCancelCallback = cancel;


		chooseItemsWnd.recipeInstance = new RecipeInstance();
		chooseItemsWnd.recipeInstance.Prototype = recipe;

		WC.AddWindow(chooseItemsWnd);


		return true;

	}

	void OnGUI()
	{
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, WC.TransformVector);
		//Map  box
		GUI.Box(new Rect(0,WC.NHeight-mapHeight,panelWidth,mapHeight),"");
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
			LF.Info("OnPlaced");
			leftPanelWnd.Show=true;
			state= Modes.Idle;
		}
	}

	public void OnDeselect()
	{
		if(state==Modes.Idle)
		{
			SelectedObject = null;
			LF.Info("OnDeselect");
		}
	}
	// Update is called once per frame
	void Update () {
	
	}

	void CallOnSelected(GameObject obj)
	{
		Component[] components = obj.GetComponents<Component>();
		foreach (Component c in components)
		{
			if(c is IInteractive)
			{
				(c as IInteractive).OnSelected();
			}
		}
	}

	void CallOnDeselected(GameObject obj)
	{
		Component[] components = obj.GetComponents<Component>();
		foreach (Component c in components)
		{
			if(c is IInteractive)
			{
				(c as IInteractive).OnDeselected();
			}
		}
	}


}
