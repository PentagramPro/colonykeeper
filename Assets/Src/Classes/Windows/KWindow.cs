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

	public abstract Results Draw();
	public abstract void Init();
}


