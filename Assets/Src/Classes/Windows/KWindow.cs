using UnityEngine;
using System;

public abstract class KWindow
{
	public enum Results{
		NoResult, Close, Ok
	}

	Rect WindowRect;
	Action<Results> OnResult;
	public Manager M;
	public WindowController WindowController;

	public KWindow(Rect windowRect, Action<Results> onResult)
	{
		WindowRect = windowRect;
		OnResult = onResult; 

	}



	public void Draw()
	{
		GUILayout.BeginArea(WindowRect);
		OnDraw();
		GUILayout.EndArea();
	}

	protected void Close(Results result)
	{
		if(OnResult!=null)
			OnResult(result);
		WindowController.RemoveWindow(this);
	}

	protected abstract void OnDraw();
	public abstract void Init();
}


