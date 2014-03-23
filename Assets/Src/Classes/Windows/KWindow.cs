using UnityEngine;
using System;

public abstract class KWindow
{
	public enum Results{
		NoResult, Close, Ok
	}

	protected Rect WindowRect;
	Action<Results> OnResult;
	bool show = true;
	public Manager M;
	public WindowController WindowController;

	public KWindow(Rect windowRect, Action<Results> onResult)
	{
		WindowRect = windowRect;
		OnResult = onResult; 

	}

	public bool Show
	{
		get{
			return show;
		}
		set{
			show = value;
		}
	}

	public void Draw()
	{
		if(show)
		{
			GUI.Box(WindowRect,"");
			GUILayout.BeginArea(WindowRect);
			OnDraw();
			GUILayout.EndArea();
		}
	}

	protected void Close(Results result)
	{
		if(OnResult!=null)
			OnResult(result);
		WindowController.RemoveWindow(this);
	}

	public static Rect ContractRect(Rect src, float amount)
	{
		return new Rect(src.x+amount,src.y+amount,src.width-amount*2, src.height-amount*2);
	}
	/*
	public Rect FromLocal(Rect rct)
	{
		return FromLocal(rct.x,rct.y,rct.width,rct.height);
	}
	public Rect FromLocal(float x, float y, float width, float height)
	{
		return new Rect(x+WindowRect.x,y+WindowRect.y,width,height);
	}

	protected void BeginArea(float x, float y, float width, float height)
	{
		GUILayout.BeginArea(new Rect(x+WindowRect.x,y+WindowRect.y,width,height));
	}
*/
	protected abstract void OnDraw();
	public abstract void Init();
}


