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
		GUILayout.TextArea("Silo");
	}
	#endregion
}
