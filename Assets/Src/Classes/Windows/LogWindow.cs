using UnityEngine;
using System;
using System.Collections.Generic;

public class LogWindow : KWindow
{

	float collapsedSize = 50f;
	float fullSize = 400f;
	bool collapsed = true;

	List<string> logHistory = new List<string>();

	public LogWindow(Rect windowRect) : base(windowRect,null)
	{
	}

	#region implemented abstract members of KWindow

	protected override void OnDraw ()
	{
		if(GUI.Button(new Rect(WindowRect.width*0.9f,0,WindowRect.width*0.1f,collapsedSize),"O"))
		{
			if(collapsed)
			{
				WindowRect.y=Screen.height-fullSize;
				WindowRect.height=fullSize;
			}
			else
			{
				WindowRect.y=Screen.height-collapsedSize;
				WindowRect.height=collapsedSize;
			}
			collapsed = !collapsed;
		}
		if(collapsed)
		{

		}
		else
		{

		}
	}

	public override void Init ()
	{

	}

	#endregion


}


