using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IngredientItemController : MonoBehaviour {

	public Text IngredientName, IngredientClass;
	IListItem selectedItem;
	Ingredient ingredient;

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
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
