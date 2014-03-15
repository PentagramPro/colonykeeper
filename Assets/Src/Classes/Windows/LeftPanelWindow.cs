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


		BeginArea(0,0,WindowRect.width,WindowRect.height*0.8f);
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
			string date = string.Format("{0,2}.{1,2}.{2,4} {3,2}:{4,2}",M.GameDateTime.Day,
			                            M.GameDateTime.Month,M.GameDateTime.Year,
			                            M.GameDateTime.Hour,M.GameDateTime.Minute);
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
		BeginArea(0,WindowRect.height*0.8f,WindowRect.width,WindowRect.height*0.2f);
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


