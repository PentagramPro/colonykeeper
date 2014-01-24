using UnityEngine;
using System.Collections;

public class FurnaceController : InOutInventory, IInteractive {

	BuildingController building;
	float targetQuantity = 0;
	Recipe targetRecipe;
	Vector2 scroll = new Vector2(0,0);
	// Use this for initialization
	void Start () {
		building = GetComponent<BuildingController>();
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
		string prod = "";


		GUILayout.Label("Production: "+prod);
		GUILayout.Button("+");
		GUILayout.Button("-");
		scroll = GUILayout.BeginScrollView(scroll);
		foreach (Recipe r in M.GameD.RecipesByDevice[building.Prototype.Name])
		{
			GUILayout.Button(r.Name);
		}
		GUILayout.EndScrollView();

	}

	#endregion
}
