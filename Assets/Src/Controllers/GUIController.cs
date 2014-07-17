using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, BuildChoose, BuildIngredients, BuildPlace, Info, Ingredients
	}
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

	Action<RecipeInstance, int> itemCallback;

	public delegate void PickedDelegate(Building pickedBuilding);

	public event PickedDelegate ItemPicked;


    public Canvas StrategyScreen;
    public BuildingScreenController BuildingScreen;
    public ItemScreenController ItemScreen;
	public HullPanelController HullPanel;

	// links to various pannels
	public StoragePanelController StoragePanel;
	public FactoryPanelController FactoryPanel;
	public ProductionPanelController ProductionPanel;

	public HullController SelectedObject{
		set{
			CallOnDeselected(HullPanel.HullToDisplay);
			HullPanel.HullToDisplay = value;
			if(value!=null)
			{
				HullPanel.gameObject.SetActive(true);
			}
			else
			{
				HullPanel.gameObject.SetActive(false);
			}
			CallOnSelected(value);
			/*if(value!=null)
				LF.Info("SelectedObject is set to "+value.name);
            if (leftPanelWnd == null)
                return;

			if(leftPanelWnd.SelectedObject!=null)
				CallOnDeselected(leftPanelWnd.SelectedObject);
			leftPanelWnd.SelectedObject = value;
			if(value!=null)
				CallOnSelected(value);*/
		}
	}

	public RecipeInstance LastRecipeInstance{
		get{
			return ItemScreen.RecipeInst;
		}
	}


	// Use this for initialization
	void Start () {
		LF.Info("============ GUI Start procedure ============");

        
        BuildingScreen.gameObject.SetActive(false);
	}


    public void OnStrategyBuild()
    {
        if (state == Modes.Idle)
        {
            StrategyScreen.gameObject.SetActive(false);
            BuildingScreen.gameObject.SetActive(true);
            state = Modes.BuildChoose;
        }
    }

    public void OnStrategyBuildCancel()
    {
        if (state ==  Modes.BuildChoose || state == Modes.BuildIngredients || state==Modes.Ingredients)
        {
            BuildingScreen.gameObject.SetActive(false);
			ItemScreen.gameObject.SetActive(false);
            StrategyScreen.gameObject.SetActive(true);
            state = Modes.Idle;
        }
    }

    public void OnStrategyBuildSelected()
    {
        if (state == Modes.BuildChoose)
        {
            if (BuildingScreen.SelectedBuilding == null)
            {
                OnStrategyBuildCancel();
            }
            else
            {
                ItemScreen.RecipeInst = new RecipeInstance();
                ItemScreen.RecipeInst.Prototype = BuildingScreen.SelectedBuilding.recipe;
                BuildingScreen.gameObject.SetActive(false);
				ItemScreen.UseQuantity = false;
                ItemScreen.gameObject.SetActive(true);
                state = Modes.BuildIngredients;
            }
            
        }
    }

	public void RequestItemsForRecipe(Recipe r, Action<RecipeInstance, int> callback)
	{
		if (state == Modes.Idle)
		{
			ItemScreen.RecipeInst = new RecipeInstance();
			ItemScreen.RecipeInst.Prototype = r;
			ItemScreen.UseQuantity = true;
			ItemScreen.gameObject.SetActive(true);
			itemCallback = callback;
			state = Modes.Ingredients;
		}
	}

	public void OnItemsForBuildingReady()
	{
		if(state==Modes.BuildIngredients)
		{

			if(ItemPicked!=null)
				ItemPicked(BuildingScreen.SelectedBuilding);
			state = Modes.BuildPlace;
			ItemScreen.gameObject.SetActive(false);
		}
		else if(state == Modes.Ingredients)
		{
			if(itemCallback!=null)
				itemCallback(ItemScreen.RecipeInst, ItemScreen.Quantity);
			ItemScreen.gameObject.SetActive(false);
			state = Modes.Idle;
		}
	}

    public void OnStrategyInfo()
    {

    }

    public void OnStrategyMenu()
    {

    }

	public void DisplayMessage(string message, Vector3 pos, Color color)
	{
		//logWnd.DisplayMessage(message,pos,color);
	}

	

	public bool GetItemsForRecipe(Recipe recipe, Action<RecipeInstance> callback,  Action cancel )
	{
		
		return true;

	}

	

	public void OnPlaced()
	{
		StrategyScreen.gameObject.SetActive(true);
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

	void CallOnSelected(HullController obj)
	{
		if(obj==null)
			return;
		Component[] components = obj.GetComponents<Component>();
		foreach (Component c in components)
		{
			if(c is IInteractive)
			{
				(c as IInteractive).OnSelected();
			}
		}
	}

	void CallOnDeselected(HullController obj)
	{
		if(obj==null)
			return;
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
