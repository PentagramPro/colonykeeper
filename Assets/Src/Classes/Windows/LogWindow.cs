using UnityEngine;
using System;
using System.Collections.Generic;

public class LogWindow : KWindow
{
	class LogRecord{
		public LogRecord(string m, Color c){
			message = m;
			color = c;
		}
		public string message;
		public Color color;
	}

	Rect rectCollapsed, rectExpanded;
	bool collapsed = true;

	List<LogRecord> logHistory = new List<LogRecord>();
	LogRecord lastMessage;

	Vector2 scroll;

	public LogWindow(Rect windowRectCollapsed, Rect windowRectExpanded) : base(windowRectCollapsed,null)
	{
		rectCollapsed = windowRectCollapsed;
		rectExpanded = windowRectExpanded;
	}

	#region implemented abstract members of KWindow

	protected override void OnDraw ()
	{
		if(GUI.Button(new Rect(WindowRect.width*0.9f,0,WindowRect.width*0.1f,rectCollapsed.height),"O"))
		{
			if(collapsed)
			{
				WindowRect=rectExpanded;
				//scroll.y = 1000;
			}
			else
			{
				WindowRect=rectCollapsed;
			}
			collapsed = !collapsed;
		}
		GUILayout.BeginArea(new Rect(10,10,WindowRect.width-20,WindowRect.height-20));
		if(collapsed)
		{
			if(lastMessage!=null)
			{
				GUI.color = lastMessage.color;
				GUILayout.Label(lastMessage.message);
			}
		}
		else
		{
			scroll = GUILayout.BeginScrollView(scroll);

			foreach(LogRecord s in logHistory)
			{
				GUI.color = s.color;
				GUILayout.Label(s.message);
			}

			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}

	public override void Init ()
	{

	}

	#endregion

	public void DisplayMessage(string message, Vector3 pos, Color color)
	{
		LogRecord l = new LogRecord(message, color);
		logHistory.Add(l);
		if(logHistory.Count>30)
			logHistory.RemoveAt(0);
		lastMessage = l;
	}

}


