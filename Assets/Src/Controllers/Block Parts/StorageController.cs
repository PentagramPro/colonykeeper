using UnityEngine;
using System.Collections;

public class StorageController : IInventory, IInteractive {


	
	#region IInteractive implementation
	public void OnSelected()
	{
		
	}
	
	public void OnDeselected()
	{
		
	}

	public override int Put (Item type, int quantity)
	{
		int left = base.Put (type, quantity);

		if(left<quantity)
			FloatingTextController.SpawnText("+"+(quantity/100).ToString("n2")+" "+type.Name, transform.position);
		return left;
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
