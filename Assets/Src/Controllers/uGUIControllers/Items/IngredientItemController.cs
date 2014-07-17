using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IngredientItemController : MonoBehaviour {

	public Text IngredientName, IngredientClass;
	IListItem selectedItem;
	Ingredient ingredient;
	Image bg;
	Color defColor;
	public Color SelectionColor;

	public IListItem SelectedItem
	{
		set
		{
			selectedItem = value;
			if(value==null)
				IngredientName.text = "No Ingredient";
			else
				IngredientName.text = value.GetName();

		}
		get
		{
			return selectedItem;
		}
	}

	public Ingredient Ingredient
	{
		set
		{
			ingredient= value;
			if(value!=null)
			{
				if(value.ClassName!="")
					IngredientClass.text = value.ClassName;
				else
					IngredientClass.text = value.Items[0].GetName();

			}
		}
		get
		{
			return ingredient;
		}
	}

	void Awake()
	{
		if(bg==null)
		{
			bg = GetComponent<Image>();
			defColor = bg.color;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Select()
	{
		if(bg==null)
		{
			bg = GetComponent<Image>();
			defColor = bg.color;
		}
		bg.color = SelectionColor;
	}

	public void Deselect()
	{
		bg.color = defColor;
	}
}
