﻿using UnityEngine;
using System.Collections;

public class BuildingController : BaseController {

	// Use this for initialization
	void Start () {
	
	}

	void OnDrawGizmos()
	{
		Vector3 center = transform.position+new Vector3(0,0.5f,0);
		Gizmos.DrawWireCube(center,new Vector3(1,1,1));
		Gizmos.DrawWireSphere(transform.position,0.2f);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
