using UnityEngine;
using System.Collections;

public class SCTipClosed : ScriptConditionBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnTipClosed ()
	{
		base.OnTipClosed ();
		Check();
	}
}
