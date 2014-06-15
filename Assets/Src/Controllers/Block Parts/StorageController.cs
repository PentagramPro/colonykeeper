using UnityEngine;
using System.Collections;

public class StorageController : IInventory, IInteractive {


	public bool ShowNotifications = true;
	#region IInteractive implementation
	public void OnSelected()
	{
		
	}
	
	public void OnDeselected()
	{
		
	}



	public void OnDrawSelectionGUI()
	{
		Item[] it = GetItemTypes();
		GUILayout.Label("Items:");
		foreach(Item item in it)
		{
			GUILayout.Label(item.Name+": "+(GetItemQuantity(item)/100.0f).ToString("n2"));
		}

		
	}
	#endregion
}
