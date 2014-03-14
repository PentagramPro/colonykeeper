using UnityEngine;
using System;
using System.Collections.Generic;

public class ChooseItemsWindow : KWindow
{

	bool prepare = true;
	public RecipeInstance recipeInstance;
	public Action<RecipeInstance> recipeCallback;
	public Action recipeCancelCallback;

	int curItem = 0;
	List<Item> itemsCache = new List<Item>();
	Vector2 infoWindowScroll = new Vector2();

	public ChooseItemsWindow(Manager m) : base(m)
	{
	}

	public override void Init()
	{
		prepare = true;
		curItem = 0;
	}
	protected override Results OnDraw()
	{
		Results res = Results.NoResult;
		Ingredient ingredient = recipeInstance.Prototype.IngredientsLinks [curItem];
	
		if (prepare)
		{
			itemsCache.Clear();
		
		
		
			M.Stat.GetItemsForIngredient(ingredient, itemsCache);
		
			prepare = false;
		}
	
		Item selected = null;
		GUILayout.Label("Choose item for ingredient #" + (curItem + 1));
		GUILayout.BeginScrollView(infoWindowScroll);
	
		foreach (Item item in itemsCache)
		{
			if (GUILayout.Button(item.Name))
				selected = item;
		}
	
		GUILayout.EndScrollView();
	
		GUILayout.BeginHorizontal();
	
		//GUILayout.Button("Build");
		if (GUILayout.Button("Cancel"))
		{
			res = Results.Close;
			if (recipeCancelCallback != null)
				recipeCancelCallback();
		}
	
		GUILayout.EndHorizontal();
	
		if (selected != null)
		{
			recipeInstance.Ingredients.Add(new Pile(selected, ingredient.Quantity));
			curItem++;
			if (curItem >= recipeInstance.Prototype.IngredientsLinks.Count)
			{
				res = Results.Ok;
				recipeCallback(recipeInstance);
			
			} else
			{
				prepare = true;
			}
		}
		return res;
	}
}


