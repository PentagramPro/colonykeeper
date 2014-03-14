using UnityEngine;
using System.Collections.Generic;
using System;

public class WindowController : BaseManagedController {

	List<KWindow> Windows = new List<KWindow>();
	List<KWindow> windowsToRemove = new List<KWindow>();
	bool isInCycle = false;

	public void AddWindow(KWindow window)
	{
		if(!Windows.Contains(window))
		{
			AddWindow(window);
			window.M = M;
			window.WindowController = this;
			window.Init();
		}

	}

	public void RemoveWindow(KWindow window)
	{
		if(isInCycle)
			windowsToRemove.Add(window);
		else
			Windows.Remove(window);
	}

	void OnGUI()
	{
		isInCycle = true;
		foreach(KWindow w in Windows)
		{
			w.Draw();
		}
		isInCycle = false;

		foreach(KWindow w in windowsToRemove)
			Windows.Remove(w);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
