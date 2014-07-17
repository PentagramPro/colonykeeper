using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PageListController))]
public class ItemScreenController : BaseManagedController {
    PageListController pageController;

    public RecipeInstance RecipeInst { get; set; }
    List<Item> itemsCache = new List<Item>();
    int curIngredient = 0;

	public List<IngredientItemController> IngredientSlots;

	public Button BtnPlus, BtnMinus;
	public Text TxtQuantity;
	public Text ProductNameObject;

	int quantity = 1;
	public int Quantity{get{return quantity;}}

	[System.NonSerialized]
	public bool UseQuantity = false;
	IngredientItemController lastSelectedItem;

	protected override void Awake ()
	{
		base.Awake ();
		pageController = GetComponent<PageListController>();
		pageController.OnItemSelected+=OnItemSelected;
		BtnPlus.onClick.AddListener(PlusMinus);
		BtnMinus.onClick.AddListener(PlusMinus);
		foreach(IngredientItemController i in IngredientSlots)
		{
			i.GetComponent<Button>().onClick.AddListener(OnIngredientSlotClick);
		}
	}
	// Use this for initialization
	void Start () {
	}

    void OnEnable()
    {
		quantity = 1;
		curIngredient = 0;
		UpdateQuantity();

		if(lastSelectedItem!=null)
			lastSelectedItem.Deselect();
		lastSelectedItem = IngredientSlots[0];
		lastSelectedItem.Select();

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
        if (M != null && M.Stat != null && RecipeInst != null)
        {
			FillListByIndex(curIngredient);
			foreach(IngredientItemController i in IngredientSlots)
				i.gameObject.SetActive(false);

			int n=0;
			foreach(Ingredient i in RecipeInst.Prototype.IngredientsLinks)
			{
				IngredientItemController item = IngredientSlots[n];
				item.gameObject.SetActive(true);
				item.SelectedItem = null;
				item.Ingredient = i;
				n++;
			}
        }



		BtnPlus.gameObject.SetActive(UseQuantity);
		BtnMinus.gameObject.SetActive(UseQuantity);
		TxtQuantity.gameObject.SetActive(UseQuantity);
		ProductNameObject.text = RecipeInst.Name;

    }

	void FillListByIndex(int index)
	{

		itemsCache.Clear();
		M.Stat.GetItemsForIngredient(RecipeInst.Prototype.IngredientsLinks[curIngredient], itemsCache);

		pageController.ItemsToDisplay.Clear();
		foreach (Item i in itemsCache)
		{
			pageController.ItemsToDisplay.Add(i);
		}
		pageController.UpdateList();
		
	}
	// Update is called once per frame
	void Update () {
		TxtQuantity.text = quantity.ToString();		
	}

	void UpdateQuantity()
	{
	}
	void PlusMinus(Button b)
	{
		if(b==BtnPlus)
			quantity++;
		else
		{
			if(quantity>1)
				quantity--;
		}
		UpdateQuantity();
	}

	void OnItemSelected(IListItem item)
	{
		IngredientSlots[curIngredient].SelectedItem = item;
	}

	void OnIngredientSlotClick(Button b)
	{
		IngredientItemController item = b.GetComponent<IngredientItemController>();
		curIngredient = IngredientSlots.IndexOf(item);
		FillListByIndex(curIngredient);

		if(lastSelectedItem!=item)
		{
			if(lastSelectedItem!=null)
				lastSelectedItem.Deselect();
			item.Select();
			lastSelectedItem = item;
		}
	}

	public void OnNextIngredient()
	{
		/*
		if(pageController.SelectedItem==null)
			return;
		int q = RecipeInst.Prototype.IngredientsLinks[curIngredient].Quantity;
		RecipeInst.Ingredients.Add(new Pile(pageController.SelectedItem as Item,
		                                    q));
		curIngredient++;
		if(curIngredient>=RecipeInst.Prototype.IngredientsLinks.Count)
		{
			M.GetGUIController().OnItemsForBuildingReady();
		}
		else
		{
			InitWindow();
		}*/


		RecipeInst.Ingredients.Clear();
		foreach(IngredientItemController item in IngredientSlots)
		{
			if(item.SelectedItem==null)
				continue;
			int q = item.Ingredient.Quantity;
			RecipeInst.Ingredients.Add(new Pile(item.SelectedItem as Item,q));

	
		}

		if(RecipeInst.Ingredients.Count == RecipeInst.Prototype.Ingredients.Count)
		{
			M.GetGUIController().OnItemsForBuildingReady();
		}
	}



}
