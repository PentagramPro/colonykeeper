using UnityEngine;
using System.Collections;

public class ColonyScreenController : BaseManagedController {


	public PageListController ResourcesList;

	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		ResourcesList.ItemsToDisplay = M.Stat.ItemsList;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
