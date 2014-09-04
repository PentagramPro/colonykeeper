using UnityEngine;
using System.Collections;

public class SCItemProduced : ScriptConditionBase {
	public FurnaceController LinkedFurnace;

	// Use this for initialization
	void Start () {
		if(LinkedFurnace!=null)
			LinkedFurnace.OnItemProduced+=OnItemProduced;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnItemProduced()
	{
		Check(	);
	}
}
