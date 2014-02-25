using UnityEngine;
using System.Collections;

public class StorageController : IInventory, IInteractive {


	
	#region IInteractive implementation
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
