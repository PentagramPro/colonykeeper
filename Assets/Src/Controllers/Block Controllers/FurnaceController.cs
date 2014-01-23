using UnityEngine;
using System.Collections;

public class FurnaceController : InOutInventory, IInteractive {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(InQuantity>0)
		{

		}
	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI ()
	{
		GUILayout.Label("Production");
		GUILayout.Button("+");
		GUILayout.Button("-");
	}

	#endregion
}
