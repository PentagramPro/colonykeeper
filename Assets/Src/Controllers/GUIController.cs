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

	public delegate void PickedDelegate(Building pickedBuilding, RecipeInstance recipe);

	public event PickedDelegate ItemPicked;


    public Canvas StrategyScreen;
    public BuildingScreenController BuildingScreen;
    public ItemScreenController ItemScreen;

	

	public GameObject SelectedObject{
		set{
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
			//return chooseItemsWnd.recipeInstance;
            return null;
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
        if (state ==  Modes.BuildChoose)
        {
            BuildingScreen.gameObject.SetActive(false);
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
                ItemScreen.gameObject.SetActive(true);
                state = Modes.BuildIngredients;
            }
            
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
