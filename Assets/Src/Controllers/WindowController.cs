using UnityEngine;
using System.Collections.Generic;
using System;

public class WindowController : BaseManagedController {


	float native_width = 1200;
	float native_height  = 800;
	Vector3 transform_vector;

	public float NWidth
	{
		get{ return native_width;}
	}
	public float NHeight
	{
		get{ return native_height;}
	}
	public Vector3 TransformVector
	{
		get{ return transform_vector;}
	}

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

		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, transform_vector);


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
		float rx  = Screen.width / native_width;
		float ry  = Screen.height / native_height;
		transform_vector = new Vector3(rx, ry, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
