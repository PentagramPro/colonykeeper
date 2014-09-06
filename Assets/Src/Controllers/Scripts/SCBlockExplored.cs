using UnityEngine;
using System.Collections;

public class SCBlockExplored : ScriptConditionBase {

	public BlockController LinkedBlock;

	// Use this for initialization
	void Start () {
		if(LinkedBlock!=null)
		{
			LinkedBlock.OnDiscovered+=OnDiscovered;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDiscovered()
	{
		Check();
	}
}
