using UnityEngine;
using System.Collections;

public class SiloController : SingleInventory, IInteractive {


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseUpAsButton()
	{
		M.GetGUIController().SelectedObject = this;
	}

	#region IInteractive implementation
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
