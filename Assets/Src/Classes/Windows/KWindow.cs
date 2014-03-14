using UnityEngine;

public abstract class KWindow
{
	public enum Results{
		NoResult, Close, Ok
	}
	protected Manager M;
	public KWindow(Manager manager)
	{
		M = manager;
	}



	public Results Draw()
	{
		Results res;


		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		res = OnDraw();
		GUILayout.EndArea();



		return res;
	}

	protected abstract Results OnDraw();
		public abstract void Init();
}


