using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FactoryItemController : BaseManagedController, IListItemAdapter {

	public Text Name;
	public Text Description;
	Image bg;
	Button button;
	Color defColor;
	// Use this for initialization
	void Start () {
		bg = GetComponent<Image>();
		button = GetComponent<Button>();
		defColor = bg.color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IListItemAdapter implementation

	public void SetListItem (IListItem item)
	{
		Name.text = item.GetName();
		Recipe r = item as Recipe;
		string descr = "";

		foreach(Ingredient i in r.IngredientsLinks)
		{
			if(i.ClassName!="")
				descr+="any "+i.ClassName+", ";
			else
				descr+=i.Items[0].Name+", ";
		}
		Description.text = descr;
	}

	public void Activate ()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate ()
	{
		gameObject.SetActive(false);
	}

	public void Select ()
	{
		bg.color = Color.green;
	}

	public void Deselect ()
	{
		if(bg!=null)
			bg.color = defColor;
	}

	public Button GetButton()
	{
		return button;
	}

	#endregion
}
