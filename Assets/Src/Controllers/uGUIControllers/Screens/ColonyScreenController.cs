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
		if(item==null || !(item is CombinedPile))
		{
			PropertiesList.UpdateList();
			return;
		}

		CombinedPile cp = item as CombinedPile;
		Pile p = cp.FirstPile;
		foreach(Field f in p.Properties.PropertiesList)
		{
			PropertiesList.ItemsToDisplay.Add(f);
		}

		PropertiesList.UpdateList();
	}
}
