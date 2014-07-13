using UnityEngine;
using System.Collections;

public class StoragePanelController : BaseManagedController {

	PageListController adapter;
	public StorageController TargetStorage {get;set;}
	// Use this for initialization
	void Start () {


	}

	void OnEnable()
	{
		adapter = GetComponent<PageListController>();
		UpdateGui();

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateGui()
	{
		Pile[] items = TargetStorage.Items;
		adapter.ItemsToDisplay.Clear();
		foreach(Pile i in items)
			adapter.ItemsToDisplay.Add(i);
		adapter.UpdateList();
	}


}
