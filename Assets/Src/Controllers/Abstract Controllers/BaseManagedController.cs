using UnityEngine;
using System;
using System.Collections;

public class BaseManagedController : BaseController {

	public GameObject manager;
	Manager managerController;

	UidContainer uidc;

	public int GetUID()
	{
		return uidc.UID;
	}
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
		uidc = new UidContainer(this);
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

	public virtual void SaveUid(WriterEx b)
	{
		uidc.Save(b);
	}

	public virtual void LoadUid(Manager m, ReaderEx r)
	{
		uidc.Load(M,r);
	}


}
