using UnityEngine;
using System.Collections;

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
