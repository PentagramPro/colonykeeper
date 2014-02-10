using UnityEngine;
using System;
using System.Collections;

public class BaseManagedController : BaseController {

	public GameObject manager;
	Manager managerController;

	public void PrepareManager()
	{
		if (manager == null) 
		{
			manager = GameObject.Find("Manager");
			if(manager==null)
				throw new UnityException("Cannot find manager object!");
		}
		if(managerController==null)
			managerController = manager.GetComponent<Manager>();
	}
	void Awake()
	{
		PrepareManager();
	}
	// Use this for initialization
	void Start () {

	}

	public Manager M
	{
		get
		{
			return managerController;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
