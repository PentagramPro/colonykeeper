using UnityEngine;
using System.Collections;

public class SCDestroyed : ScriptConditionBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		Check();
	}
}
