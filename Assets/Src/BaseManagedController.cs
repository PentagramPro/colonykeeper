using UnityEngine;
using System;
using System.Collections;

public class BaseManagedController : BaseController {

	public GameObject manager;
	Manager managerController;
	// Use this for initialization
	void Start () {
		if(manager==null)
			throw new Exception("Manager field is not set!");
		managerController = manager.GetComponent<Manager>();
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
