using UnityEngine;
using System.Collections;

public class StorageController : IInventory, IInteractive {

	[System.NonSerialized]
	public StoragePanelController StoragePanel;
	public bool ShowNotifications = true;

	void Start()
	{
		//StoragePanel  = GameObject.Find ("StoragePanel",false).GetComponent<StoragePanelController>();
		StoragePanel = M.GetGUIController().StoragePanel;
	}

	void Update()
	{

	}
	#region IInteractive implementation
	public void OnSelected()
	{
		if(StoragePanel!=null)
		{
			StoragePanel.TargetStorage = this;
			StoragePanel.gameObject.SetActive(true);	
		}
	}
	
	public void OnDeselected()
	{
		if(StoragePanel!=null)
		{
			StoragePanel.TargetStorage = null;
			StoragePanel.gameObject.SetActive(false);	
		}

	}



	public void OnDrawSelectionGUI()
	{
		/*
		Item[] it = GetItemTypes();
		GUILayout.Label("Items:");
		foreach(Item item in it)
		{
			GUILayout.Label(item.Name+": "+(GetItemQuantity(item)/100.0f).ToString("n2"));
		}
*/
		
	}
	#endregion
}
