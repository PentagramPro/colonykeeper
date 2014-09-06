using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ScriptNodeController))]
public class SAEndLevel : BaseManagedController, IScriptAction {

	public bool victory = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IScriptAction implementation

	public void Execute ()
	{
		M.FinishLevel(victory);
	}

	#endregion
}
