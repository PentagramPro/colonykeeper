using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ScriptNodeController))]
public class SCMined : ScriptConditionBase {
	public BlockController LinkedBlock;


	// Use this for initialization
	void Start () {
		if(LinkedBlock!=null)
			LinkedBlock.OnMined+=OnMined;


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMined()
	{
		
		Check();
	}
}
