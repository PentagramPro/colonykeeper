using System;
using UnityEngine;

public class LeftPanelWindow : KWindow
{


	Action OnBuild, OnInfo;

	public GameObject SelectedObject;

	public LeftPanelWindow (Rect windowRect,Action onBuild, Action onInfo) : base(windowRect,null)
	{
		this.OnBuild = onBuild;
		this.OnInfo = onInfo;
	}
	

	#region implemented abstract members of KWindow

	protected override void OnDraw ()
	{


		//selection


		GUILayout.BeginArea(new Rect(0,0,WindowRect.width,WindowRect.height*0.8f));
		if(SelectedObject!=null)
		{
			Component[] items = SelectedObject.GetComponents<Component>();
			
			foreach(Component item in items)
			{
				if(item is IInteractive)
					((IInteractive)item).OnDrawSelectionGUI();
			}
		}
		else
		{
			string date = M.GameDateTime.ToString("HH:mm, d MMM yyyy");
			GUILayout.Label(date);
			
			if(GUILayout.Button("Save"))
			{
				M.SaveGame();
			}
			if(GUILayout.Button("Load"))
			{
				M.LoadGame();
			}
		}

		GUILayout.EndArea();

		// toolbar
		GUILayout.BeginArea(new Rect(0,WindowRect.height*0.8f,WindowRect.width,WindowRect.height*0.2f));
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Build"))
			OnBuild();
		
		if(GUILayout.Button("Info"))
			OnInfo();
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();


	}

	public override void Init ()
	{

	}

	#endregion


}


