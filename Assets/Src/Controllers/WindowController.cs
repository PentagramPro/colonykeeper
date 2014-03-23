using UnityEngine;
using System.Collections.Generic;
using System;

public class WindowController : BaseManagedController {

	List<KWindow> Windows = new List<KWindow>();
	List<KWindow> windowsToRemove = new List<KWindow>();
	List<KWindow> windowsToAdd = new List<KWindow>();
	bool isInCycle = false;

	public GUISkin Skin;

	public void AddWindow(KWindow window)
	{
		if(isInCycle)
			windowsToAdd.Add(window);
		else
			AddWindowInternal(window);

	}

	void AddWindowInternal(KWindow window)
	{
		if(!Windows.Contains(window))
		{
			Windows.Add(window);
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
		if(Skin!=null)
			GUI.skin = Skin;

		GUI.color = Color.white;
		isInCycle = true;
		foreach(KWindow w in Windows)
		{
			w.Draw();
		}
		isInCycle = false;

		if(windowsToRemove.Count>0)
		{
			foreach(KWindow w in windowsToRemove)
				Windows.Remove(w);
			windowsToRemove.Clear();
		}
		if(windowsToAdd.Count>0)
		{
			foreach(KWindow w in windowsToAdd)
				AddWindowInternal(w);
			windowsToAdd.Clear();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
