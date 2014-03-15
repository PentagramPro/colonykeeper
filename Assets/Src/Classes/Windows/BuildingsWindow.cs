using UnityEngine;
using System;

public class BuildingsWindow : KWindow
{
	Action<Building> OnBuild;
	Vector2 scroll = new Vector2();
	public BuildingsWindow (Rect windowRect, Action<Building> onBuild) : base(windowRect,null)
	{
		OnBuild = onBuild;
	}


	#region implemented abstract members of KWindow
	protected override void OnDraw ()
	{
		Building selectedBuilding = null;
		scroll = GUILayout.BeginScrollView(scroll);
		foreach(Building b in M.GameD.Buildings)
		{
			if(b.Hide)
				continue;
			if(GUILayout.Button(b.Name))
			{
				selectedBuilding = b;	
			}
		}

		if(selectedBuilding!=null)
			OnBuild(selectedBuilding);
		GUILayout.EndScrollView();
	}
	public override void Init ()
	{

	}
	#endregion
}

