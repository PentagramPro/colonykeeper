using UnityEngine;
using System.Collections;

public class StoragePanelController : BaseManagedController {

	PageListController adapter;
	public ProgressBarController CapacityIndicator;
	public StorageController TargetStorage {get;set;}
	public float Capacity{
		set
		{
			CapacityIndicator.Progress = value;
		}
	}
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
		Capacity = (float)TargetStorage.Quantity/TargetStorage.MaxQuantity;
	}


}
