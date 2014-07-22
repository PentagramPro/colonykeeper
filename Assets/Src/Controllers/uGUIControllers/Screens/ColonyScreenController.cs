using UnityEngine;
using System.Collections;

public class ColonyScreenController : BaseManagedController {


	public PageListController ResourcesList;
	public PageListController PropertiesList;

	// Use this for initialization
	void Start () {
		ResourcesList.OnItemSelected+=OnResourceItemSelected;
	}

	void OnEnable()
	{
		ResourcesList.ItemsToDisplay = M.Stat.ItemsList;

	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnResourceItemSelected(IListItem item)
	{
		PropertiesList.ItemsToDisplay.Clear();
		if(item==null || !(item is Pile))
		{
			PropertiesList.UpdateList();
			return;
		}

		Pile p = item as Pile;

		foreach(Field f in p.Properties.PropertiesList)
		{
			PropertiesList.ItemsToDisplay.Add(f);
		}

		PropertiesList.UpdateList();
	}
}
