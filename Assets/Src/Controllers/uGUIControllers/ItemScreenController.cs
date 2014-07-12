using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PageListController))]
public class ItemScreenController : BaseManagedController {
    PageListController pageController;

    public RecipeInstance RecipeInst { get; set; }
    List<Item> itemsCache = new List<Item>();
    int curIngredient = 0;


	// Use this for initialization
	void Start () {
        InitWindow();
	}

    void OnEnable()
    {
        itemsCache.Clear();
        InitWindow();
        M.BlockMouseInput = true;
    }

    void OnDisable()
    {
        M.BlockMouseInput = false;

    }
    void InitWindow()
    {
        if (pageController == null)
        {
            Debug.Log("setting up page controller");
            pageController = GetComponent<PageListController>();
        }

        if (M != null && M.Stat != null && RecipeInst != null)
        {
            Debug.Log("getting items for ingredient");
			itemsCache.Clear();
            M.Stat.GetItemsForIngredient(RecipeInst.Prototype.IngredientsLinks[curIngredient], itemsCache);
        }

		pageController.ItemsToDisplay.Clear();
        foreach (Item i in itemsCache)
        {
            pageController.ItemsToDisplay.Add(i);
        }
		pageController.UpdateList();

    }
	// Update is called once per frame
	void Update () {
	
	}

	public void OnNextIngredient()
	{
		if(pageController.SelectedItem==null)
			return;
		RecipeInst.Ingredients.Add(new Pile(pageController.SelectedItem as Item,
		                                    RecipeInst.Prototype.IngredientsLinks[curIngredient].Quantity));
		curIngredient++;
		if(curIngredient>=RecipeInst.Prototype.IngredientsLinks.Count)
		{
			M.GetGUIController().OnItemsForBuildingReady();
		}
		else
		{
			InitWindow();
		}
	}



}
