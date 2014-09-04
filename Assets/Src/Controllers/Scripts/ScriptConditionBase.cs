using UnityEngine;
using System.Collections;

public class ScriptConditionBase : BaseManagedController , IScriptCondition{
	ScriptNodeController node;
	bool check;

	protected override void Awake ()
	{
		base.Awake ();
		node = GetComponent<ScriptNodeController>();

	}


	protected void Check()
	{
		check = true;
		node.OnConditionChecked();
	}

	#region IScriptCondition implementation

	public bool IsChecked ()
	{
		return check;
	}

	public virtual void OnTipClosed ()
	{

	}

	#endregion
}
