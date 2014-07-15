using UnityEngine;
using System.Collections.Generic;

public class FactoryPanelController : BaseManagedController {

	PageListController adapter;
	public FurnaceController TargetFurnace;

	protected override void Awake()
	{
		base.Awake();
		adapter = GetComponent<PageListController>();
	}
	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		List<Recipe> list = M.GameD.RecipesByDevice[TargetFurnace.Name];
		foreach(Recipe r in list)
		{
			adapter.ItemsToDisplay.Add(r);
		}
		adapter.UpdateList();
	}

	void OnDisable()
	{
		adapter.ItemsToDisplay.Clear();
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void OnProduce()
	{
		if(adapter!=null && 
		   adapter.SelectedItem!=null &&
		   TargetFurnace!=null)
		{

		}
	}
}
