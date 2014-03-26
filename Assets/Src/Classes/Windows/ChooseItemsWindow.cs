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

	public ChooseItemsWindow(Rect windowRect, Action<Results> onResult) : base(windowRect,onResult)
	{
	}

	public override void Init()
	{
		prepare = true;
		curItem = 0;
	}
	protected override void OnDraw()
	{



		Ingredient ingredient = recipeInstance.Prototype.IngredientsLinks [curItem];
	
		if (prepare)
		{
			itemsCache.Clear();
		
		
		
			M.Stat.GetItemsForIngredient(ingredient, itemsCache);
		
			prepare = false;
		}
	
		Rect leftRect = new Rect(0,0,WindowRect.width/2,WindowRect.height);
		Rect rightRect = new Rect(WindowRect.width/2,0,WindowRect.width/2,WindowRect.height);


		//GUI.DrawTexture(leftRect,SolidTexture.GetTexture(new Color(0.2f,0.2f,0.2f)));

		GUILayout.BeginArea(leftRect);
		LeftPanel();
		GUILayout.EndArea();


		//GUI.DrawTexture(rightRect,SolidTexture.GetTexture(Color.gray));


		GUILayout.BeginArea(rightRect);
		RightPanel(ingredient);
		GUILayout.EndArea();
	}

	void LeftPanel()
	{

		GUILayout.Label(recipeInstance.Prototype.Name,skinDarkHeader);
		GUILayout.Label("This recipe needs following ingredients:");

		int n = 0;
		foreach(Ingredient i in recipeInstance.Prototype.IngredientsLinks)
		{
			string line;
			if(i.ClassName.Length>0)
				line = i.ClassName;
			else
				line = i.Items[0].Name;


			GUILayout.Label(line,n==curItem ? skinBrightListItem : skinDarkListItem);
			n++;
		}

	}

	void RightPanel(Ingredient ingredient)
	{
		Item selected = null;

		GUILayout.Label("Choose item for ingredient #" + (curItem + 1),skinBrightText);
		GUILayout.BeginScrollView(infoWindowScroll,skinBrightScroll);
		
		foreach (Item item in itemsCache)
		{
			if (GUILayout.Button(item.Name,skinBrightListItem))
				selected = item;
		}
		
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal();
		
		//GUILayout.Button("Build");
		if (GUILayout.Button("Cancel"))
		{
			Close(Results.Close);
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
				Close (Results.Ok);
				recipeCallback(recipeInstance);
				
			} else
			{
				prepare = true;
			}
		}
	}
}


