using UnityEngine;
using System.Collections;

public class LevelsMenu : BaseController {

	PageListController pageList;

	void Awake()
	{
		LevelDataController[] levels = GetComponentsInChildren<LevelDataController>();
		pageList = GetComponent<PageListController>();
		foreach(LevelDataController l in levels)
			pageList.ItemsToDisplay.Add(l);
		pageList.OnItemSelected+=OnItemSelected;
	}
	// Use this for initialization
	void Start () {
		
	}

	void OnItemSelected(IListItem item)
	{
		LevelDataController level = item as LevelDataController;

	}

	// Update is called once per frame
	void Update () {
	
	}
}
