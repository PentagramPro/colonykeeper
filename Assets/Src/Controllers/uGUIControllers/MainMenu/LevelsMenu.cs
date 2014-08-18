using UnityEngine;
using System.Collections;

public class LevelsMenu : BaseController {

	PageListController pageList;
	LevelDataController selectedLevel;
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
		selectedLevel = item as LevelDataController;

	}

	public void OnStartLevel()
	{
		if(selectedLevel!=null)
			Application.LoadLevel(selectedLevel.SceneName);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
