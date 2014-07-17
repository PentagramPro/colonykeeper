﻿using UnityEngine;
using System.Collections;

public class ClickBlocker : BaseManagedController {

	bool blocked = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter()
	{
		Block();
	}

	public void OnPointerLeave()
	{
		Unblock();
	}

	void OnDisable()
	{
		Unblock();
	}

	void Block()
	{
		if(blocked==false)
		{
			M.BlockMouseInput = true;
			blocked = true;
		}
	}

	void Unblock()
	{
		if(blocked==true)
		{
			M.BlockMouseInput = false;
			blocked = false;
		}
	}
}
