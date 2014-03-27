using UnityEngine;
using System;
using System.Collections.Generic;

public class InfoWindow : KWindow
{
	Vector2 scroll;

	public InfoWindow(Rect windowRect, Action<Results> onResult) : base(windowRect,onResult)
	{
	}
	#region implemented abstract members of KWindow

	protected override void OnDraw()
	{
		scroll = GUILayout.BeginScrollView(scroll);
		

		foreach(Item i in M.Stat.Items.Keys)
		{
			
			GUILayout.BeginHorizontal();
			skinDarkListItem.fixedWidth = WindowRect.width * 0.6f;
			GUILayout.Label(i.Name, skinDarkListItem);
			skinDarkListItem.fixedWidth = WindowRect.width * 0.2f;
			GUILayout.Label( (M.Stat.Items[i]/100.0f).ToString("n2") );
			skinDarkListItem.fixedWidth = 0;
			GUILayout.EndHorizontal();
			
		}
		GUILayout.EndScrollView();
		if(GUILayout.Button("Close"))
			Close(Results.Close);
	}

	public override void Init()
	{

	}

	#endregion


}

