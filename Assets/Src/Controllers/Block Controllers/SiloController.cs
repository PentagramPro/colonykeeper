using UnityEngine;
using System.Collections;

public class SiloController : IInventory, IInteractive {


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
		GUILayout.TextArea("Silo");
		if(it!=null && it.Length>0)
		{
			GUILayout.TextArea(it[0].Name+": "+(Quantity/100.0f).ToString("n2"));
		}

	}
	#endregion
}
