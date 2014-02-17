using UnityEngine;
using System;
using System.Collections;

public class BaseManagedController : BaseController {

	public GameObject manager;
	Manager managerController;

	private static int nextUid=0;
	private int uid=0;
	public int UID{
		get{
			return uid;
		}
	}
	private int loadedUid=0;

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
		uid = nextUid++;
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

	protected void SaveHash(WriterEx b)
	{
		b.Write(uid);
	}

	protected void LoadHash(ReaderEx r)
	{
		loadedUid = r.ReadInt32();
		M.LoadedLinks.Add(loadedUid,this);
	}


}
